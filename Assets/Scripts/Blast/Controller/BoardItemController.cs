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
        private IBoardItem[,] _boardItems;
        private bool[,] _recursiveCheckArray;
        private List<IBoardItem> _combineItems;
        private int _rowLength, _columnLength;
        private Dictionary<int, int> _spawnerLocation;

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
                AdjustBoardItems();
            }
        }

        private void AdjustBoardItems()
        {
            var levelData = _inGameController.LevelData;
            _rowLength = levelData.RowLength;
            _columnLength = levelData.ColumnLength;

            _boardItems = new IBoardItem[_rowLength, _columnLength];
            _recursiveCheckArray = new bool[_rowLength, _columnLength];
            _combineItems = new List<IBoardItem>(_rowLength * _columnLength);
            foreach (var item in levelData.BoardItem)
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

            _spawnerLocation = new Dictionary<int, int>();

            foreach (var item in levelData.SpawnerData.Spawners)
            {
                _spawnerLocation.Add(item.Column, item.Row);
            }
        }

        public void OnClick(int row, int column)
        {
            var item = _boardItems[row, column];
            if (item.IsMove)
                return;

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

        private async void BlastForPowerUp(int clickRow, int clickColumn)
        {
            var tempGroup = new List<IBoardItem>(_combineItems);
            _combineItems.Clear();
            Array.Clear(_recursiveCheckArray, 0, _recursiveCheckArray.Length);

            FillVoidType(tempGroup);
            await Combine(clickRow, clickColumn, tempGroup);
            tempGroup.ForEach(item => item.ReturnToPool());
            RecalculateBoardElements();
        }

        private async Task Combine(int clickRow, int clickColumn, List<IBoardItem> tempGroup)
        {
            CombineState combineState = null;
            foreach (var item in tempGroup)
            {
                if (item is Bead bead)
                {
                    combineState = (CombineState)item.MovementVisitor.MovementStrategy.CombineState;
                    combineState.SetParam(clickRow - item.Row, clickColumn - item.Column);
                    _movementController.Register(item, item.MovementVisitor.MovementStrategy.CombineState);
                    bead.SetLayer(item.Row, clickColumn - item.Column);
                }
            }

            await UniTask.WaitUntil(() => combineState!.AllMovementsComplete);
        }

        private void Blast()
        {
            _combineItems.ForEach(item => item.Blast());
            _combineItems.ForEach(item => item.ReturnToPool());
            FillVoidType(_combineItems);
            RecalculateBoardElements();
            _combineItems.Clear();
            Array.Clear(_recursiveCheckArray, 0, _recursiveCheckArray.Length);
        }

        private void Shake(IBoardItem item)
        {
            _movementController.Register(item, item.MovementVisitor.MovementStrategy.Shake);
            _combineItems.Clear();
            Array.Clear(_recursiveCheckArray, 0, _recursiveCheckArray.Length);
        }

        private void FillVoidType(List<IBoardItem> combineGroup)
        {
            foreach (var item in combineGroup)
            {
                _movementController.Check(_boardItems[item.Row, item.Column]);

                var garbage = _boardItems[item.Row, item.Column];
                BoardItemPool.Instance.Return(garbage);

                if (!BoardItemPool.Instance.TryRetrieveWithoutParams<VoidArea>(out var voidArea))
                {
                    voidArea = BoardItemPool.Instance.Retrieve<VoidArea>(item.Row, item.Column);
                }

                _boardItems[item.Row, item.Column] = voidArea;
                _boardItems[item.Row, item.Column].SetRowAndColumn(item.Row, item.Column);
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
            _movementController.Check(_boardItems[nonEmptyRowIndex, column]);

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

            _movementController.Register(item, item.MovementVisitor.MovementStrategy.StartMovement);
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

        private void AdjustItemPosition(int column, int row, int distanceToNextBead, ref float verticalOffset,
            Bead bead)
        {
            var offsetRow = 0;
            for (int i = row - 1; i >= 0; i--)
            {
                if (!_boardItems[row, column].IsMove) continue;

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
                _recursiveCheckArray[row, column] || _boardItems[row, column].IsMove )
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
    }
}