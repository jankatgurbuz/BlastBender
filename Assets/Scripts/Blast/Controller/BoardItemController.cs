using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BoardItems;
using BoardItems.Bead;
using BoardItems.Void;
using Cysharp.Threading.Tasks;
using Global.Controller;
using Signals;
using Util.Movement.States;
using Util.Pool.BoardItemPool;
using Zenject;
using Random = UnityEngine.Random;

namespace Blast.Controller
{
    public class BoardItemController
    {
        private readonly IInGameController _inGameController;
        private readonly IGridController _gridController;
        private readonly MovementController _movementController;

        private int _rowLength, _columnLength;

        private Dictionary<int, int> _spawnerLocation;
        private IBoardItem[,] _boardItems;
        private List<IBoardItem> _combineItems;
        private bool[,] _recursiveCheckArray;

        public BoardItemController(SignalBus signalBus, IGridController gridController,
            IInGameController inGameController, MovementController movementController)
        {
            _gridController = gridController;
            _inGameController = inGameController;
            _movementController = movementController;
            signalBus.Subscribe<GameStateReaction>(OnReaction);
        }

        private void OnReaction(GameStateReaction reaction)
        {
            if (reaction.GameStatus == GameStatus.GameInitialize)
            {
                SetupBoardItems();
                AdjustBoardItems();
                AdjustSpawnerLocation();
            }
        }

        private void SetupBoardItems()
        {
            _rowLength = _inGameController.LevelData.RowLength;
            _columnLength = _inGameController.LevelData.ColumnLength;

            _boardItems = new IBoardItem[_rowLength, _columnLength];
            _recursiveCheckArray = new bool[_rowLength, _columnLength];
            _combineItems = new List<IBoardItem>(_rowLength * _columnLength);
            _spawnerLocation = new Dictionary<int, int>();
        }

        private void AdjustBoardItems()
        {
            foreach (var item in _inGameController.LevelData.BoardItem)
            {
                var temp = _boardItems[item.Row, item.Column] = item.Copy();
                temp.RetrieveFromPool();

                if (temp is IVisual itemWithColor)
                {
                    itemWithColor.SetColorAndAddSprite(itemWithColor.Color);
                    itemWithColor.SetSortingOrder(item.Row, item.Column);
                }

                temp.TransformUtilities?.SetPosition(_gridController.CellToLocal(item.Row, item.Column));
                temp.SetActive(true);
            }
        }

        private void AdjustSpawnerLocation()
        {
            foreach (var item in _inGameController.LevelData.SpawnerData.Spawners)
            {
                _spawnerLocation.Add(item.Column, item.Row);
            }
        }

        public void OnClick(int row, int column)
        {
            var item = _boardItems[row, column];

            if (item is IMoveable { IsMove: true }) return;

            if (!item.IsBead)
            {
                item.Blast();
                return;
            }

            var color = ((Bead)item).Color;
            FindMatches(row, column, color);

            switch (_combineItems.Count)
            {
                case > 4: // todo: magic number !!
                    BlastForPowerUp(row, column);
                    break;
                case > 1:
                    Blast();
                    break;
                default:
                    Shake(item);
                    break;
            }
        }

        private void Blast()
        {
            _combineItems.ForEach(item => item.Blast());
            _combineItems.ForEach(item => item.ReturnToPool());
            _combineItems.ForEach(ReplaceWithVoidArea);

            RecalculateBoardElements();
            ClearCombineItems();
            ClearRecursiveCheckArray();
        }

        private async void BlastForPowerUp(int clickRow, int clickColumn)
        {
            var tempGroup = new List<IBoardItem>(_combineItems);

            ClearCombineItems();
            ClearRecursiveCheckArray();

            tempGroup.ForEach(ReplaceWithVoidArea);
            await Combine(clickRow, clickColumn, tempGroup);
            tempGroup.ForEach(item => item.ReturnToPool());

            RecalculateBoardElements();
        }

        private void ReplaceWithVoidArea(IBoardItem item)
        {
            RemoveIfInFinishState(item);

            var garbage = _boardItems[item.Row, item.Column];
            BoardItemPool.Instance.Return(garbage);

            if (!BoardItemPool.Instance.TryRetrieveWithoutParams<VoidArea>(out var voidArea))
            {
                voidArea = BoardItemPool.Instance.Retrieve<VoidArea>(item.Row, item.Column);
            }

            _boardItems[item.Row, item.Column] = voidArea;
            _boardItems[item.Row, item.Column].SetRowAndColumn(item.Row, item.Column);
        }

        private async Task Combine(int clickRow, int clickColumn, List<IBoardItem> tempGroup)
        {
            CombineState combineState = null;
            foreach (var item in tempGroup)
            {
                if (item is IMoveable moveableItem)
                {
                    combineState = (CombineState)moveableItem.MovementVisitor.MovementStrategy.CombineState;
                    combineState.SetParam(clickRow - item.Row, clickColumn - item.Column);
                    _movementController.Register(moveableItem,
                        moveableItem.MovementVisitor.MovementStrategy.CombineState);
                    ((Bead)moveableItem).SetLayer(item.Row, clickColumn - item.Column);
                }
            }

            await UniTask.WaitUntil(() => combineState!.AllMovementsComplete);
        }


        private void Shake(IBoardItem item)
        {
            if (item is IMoveable moveableItem)
            {
                _movementController.Register(moveableItem, moveableItem.MovementVisitor.MovementStrategy.Shake);
                ClearCombineItems();
                ClearRecursiveCheckArray();
            }
        }

        private void RecalculateBoardElements()
        {
            for (int column = 0; column < _columnLength; column++)
            {
                var isFirstVoidArea = true;
                var distanceToNextBead = 0;
                var offset = 0f;

                for (int row = 0; row < _rowLength; row++)
                {
                    if (_boardItems[row, column].IsBead) continue;
                    if (_boardItems[row, column].IsSpace) continue;

                    ShiftBeadOrSpawnNew(_boardItems[row, column], ref isFirstVoidArea,
                        ref distanceToNextBead, ref offset);
                }
            }
        }

        private void ShiftBeadOrSpawnNew(IBoardItem boardItem, ref bool isFirstVoidArea,
            ref int distanceToNextBead, ref float verticalOffset)
        {
            var column = boardItem.Column;
            var row = boardItem.Row;

            for (int nonEmptyRowIndex = row + 1; nonEmptyRowIndex <= _rowLength; nonEmptyRowIndex++)
            {
                if (nonEmptyRowIndex < _rowLength && _boardItems[nonEmptyRowIndex, column].IsBead)
                {
                    TryShiftBeadDown(nonEmptyRowIndex, row, column);
                    break;
                }

                if (nonEmptyRowIndex == _rowLength)
                {
                    SpawnNewBeadInVoidArea(column, row, ref isFirstVoidArea, ref distanceToNextBead,
                        ref verticalOffset);
                }
            }
        }

        private void TryShiftBeadDown(int nonEmptyRowIndex, int row, int column)
        {
            RemoveIfInFinishState(_boardItems[nonEmptyRowIndex, column]);

            var garbage = _boardItems[row, column];
            BoardItemPool.Instance.Return(garbage);

            var item = _boardItems[row, column] = _boardItems[nonEmptyRowIndex, column];
            item.SetRowAndColumn(row, column);
            ((IVisual)item).SetSortingOrder(row, column);

            if (!BoardItemPool.Instance.TryRetrieveWithoutParams<VoidArea>(out var voidArea))
            {
                voidArea = BoardItemPool.Instance.Retrieve<VoidArea>(nonEmptyRowIndex, column);
            }

            _boardItems[nonEmptyRowIndex, column] = voidArea;
            _boardItems[nonEmptyRowIndex, column].SetRowAndColumn(nonEmptyRowIndex, column);

            var moveableItem = (IMoveable)item;
            _movementController.Register(moveableItem, moveableItem.MovementVisitor.MovementStrategy.StartMovement);
        }

        private void SpawnNewBeadInVoidArea(int column, int row, ref bool isFirstVoidArea,
            ref int distanceToNextBead, ref float verticalOffset)
        {
            if (!_spawnerLocation.TryGetValue(column, out int location)) return;

            if (isFirstVoidArea)
            {
                distanceToNextBead = location - row;
                isFirstVoidArea = false;
            }

            var garbage = _boardItems[row, column];
            BoardItemPool.Instance.Return(garbage);

            var randomColor = (ItemColors)Random.Range(0, Enum.GetValues(typeof(ItemColors)).Length - 1);
            // randomColor = ItemColors.Red; // Test
            if (!BoardItemPool.Instance.TryRetrieveWithoutParams<Bead>(out var item))
            {
                item = BoardItemPool.Instance.Retrieve<Bead>(row, column, randomColor);
            }

            var bead = (Bead)item;
            _boardItems[row, column] = bead;

            bead.RetrieveFromPool();
            bead.SetRowAndColumn(row, column);
            bead.SetSortingOrder(row, column);
            bead.SetColorAndAddSprite(randomColor);
            bead.MovementVisitor.MovementStrategy.ResetAllStates();
            bead.SetActive(true);

            AdjustItemPosition(column, row, distanceToNextBead, ref verticalOffset, bead);
            _movementController.Register(bead, bead.MovementVisitor.MovementStrategy.StartMovement);
        }

        private void AdjustItemPosition(int column, int row, int distanceToNextBead,
            ref float verticalOffset, Bead bead)
        {
            var offsetRow = 0;
            for (int i = row - 1; i >= 0; i--)
            {
                if (_boardItems[row, column] is not IMoveable moveable) continue;
                if (!moveable.IsMove) continue;

                offsetRow = i;
                break;
            }

            var position = _gridController.CellToLocal(row + distanceToNextBead, column);
            verticalOffset += _inGameController.LevelData.SpawnerData.VerticalOffset;
            position.y += verticalOffset + offsetRow;
            bead.TransformUtilities.SetPosition(position);
        }

        private void FindMatches(int row, int column, ItemColors color)
        {
            if (row < 0 || column < 0 || row >= _rowLength || column >= _columnLength ||
                _recursiveCheckArray[row, column] || _boardItems[row, column] is not IMoveable moveableItem ||
                moveableItem.IsMove)
                return;

            _recursiveCheckArray[row, column] = true;

            if (!_boardItems[row, column].IsBead || ((Bead)_boardItems[row, column]).Color != color)
                return;

            _combineItems.Add(_boardItems[row, column]);

            FindMatches(row + 1, column, color);
            FindMatches(row - 1, column, color);
            FindMatches(row, column + 1, color);
            FindMatches(row, column - 1, color);
        }

        private void ClearCombineItems()
        {
            _combineItems.Clear();
        }

        private void ClearRecursiveCheckArray()
        {
            Array.Clear(_recursiveCheckArray, 0, _recursiveCheckArray.Length);
        }

        private void RemoveIfInFinishState(IBoardItem item)
        {
            if (_boardItems[item.Row, item.Column] is IMoveable iMoveableItem)
            {
                _movementController.RemoveIfInFinishState(iMoveableItem);
            }
        }
    }
}