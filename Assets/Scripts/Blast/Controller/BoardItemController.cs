using System;
using System.Collections.Generic;
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
        private const string SortingOrderKeyForCombineBeads = "CombineBeads";
        private readonly IInGameController _inGameController;
        private readonly IGridController _gridController;
        private readonly MovementController _movementController;

        private int _rowLength, _columnLength;

        private Dictionary<int, int> _spawnerLocation;
        private IBoardItem[,] _boardItems;
        private List<IBoardItem> _combineItems;
        private bool[,] _recursiveCheckArray;

        public IBoardItem[,] BoardItems => _boardItems;

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
                temp.BoardItemController = this;

                if (temp is ISortingOrder itemWithColor)
                {
                    itemWithColor.SetSortingOrder(item.GetType().FullName, item.Row, item.Column);
                }

                if (temp is IColorable color)
                {
                    color.Color = ((IColorable)item).Color;
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

            if (item.IsBead)
            {
                ClickBead(item, row, column);
            }
        }

        private void ClickBead(IBoardItem item, int row, int column)
        {
            if (item is IMovable { IsMoving: true }) return;

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
            foreach (var item in _combineItems)
            {
                var beadItem = (Bead)item;
                beadItem.Pop();
                beadItem.ReturnToPool();
                RemoveIfInFinishState(item);
                ReplaceWithVoidArea(item);
            }

            RecalculateBoardElements();
            ClearCombineItems();
            ClearRecursiveCheckArray();
        }

        private async void BlastForPowerUp(int clickRow, int clickColumn)
        {
            var tempGroup = new List<IBoardItem>(_combineItems);

            ClearCombineItems();
            ClearRecursiveCheckArray();

            foreach (var item in tempGroup)
            {
                RemoveIfInFinishState(item);
                ReplaceWithVoidArea(item);
            }

            await Combine(clickRow, clickColumn, tempGroup);

            foreach (var item in tempGroup)
            {
                item.ReturnToPool();
            }

            RecalculateBoardElements();
        }

        private async UniTask Combine(int clickRow, int clickColumn, List<IBoardItem> tempGroup)
        {
            //Todo interface segregation
            CombineState combineState = null;
            foreach (var item in tempGroup)
            {
                if (item is IMovable moveableItem)
                {
                    combineState = (CombineState)moveableItem.MovementStrategy.CombineState;
                    combineState.SetParam(clickRow - item.Row, clickColumn - item.Column);

                    _movementController.Register(moveableItem, moveableItem.MovementStrategy.CombineState);

                    ((Bead)moveableItem).SetSortingOrder(SortingOrderKeyForCombineBeads, item.Row,
                        clickColumn - item.Column);
                }
            }

            await UniTask.WaitUntil(() => combineState!.AllMovementsComplete);
        }


        private void Shake(IBoardItem item)
        {
            if (item is IMovable moveableItem)
            {
                _movementController.Register(moveableItem, moveableItem.MovementStrategy.Shake);
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
                    if (!_boardItems[row, column].IsVoidArea) continue;

                    ShiftBeadOrSpawnNew(row, column, ref isFirstVoidArea,
                        ref distanceToNextBead, ref offset);
                }
            }
        }

        private void ShiftBeadOrSpawnNew(int row, int column, ref bool isFirstVoidArea,
            ref int distanceToNextBead, ref float verticalOffset)
        {
            for (int nonEmptyRowIndex = row + 1; nonEmptyRowIndex <= _rowLength; nonEmptyRowIndex++)
            {
                if (nonEmptyRowIndex < _rowLength && _boardItems[nonEmptyRowIndex, column] is IMovable)
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
            // If in finish state, remove from movement list
            RemoveIfInFinishState(_boardItems[nonEmptyRowIndex, column]);

            // Send the object to the pool
            ReturnPool(_boardItems[row, column]);

            // Perform the swap operation
            var item = _boardItems[row, column] = _boardItems[nonEmptyRowIndex, column];
            item.SetRowAndColumn(row, column);
            ((ISortingOrder)item).SetSortingOrder(item.GetType().FullName, row, column);

            // The second part of the swap operation
            var voidArea = RetrievePool<VoidArea>(nonEmptyRowIndex, column);
            _boardItems[nonEmptyRowIndex, column] = voidArea;
            _boardItems[nonEmptyRowIndex, column].SetRowAndColumn(nonEmptyRowIndex, column);

            // Register for movement push
            var moveableItem = (IMovable)item;
            _movementController.Register(moveableItem, moveableItem.MovementStrategy.StartMovement);
            moveableItem.MovementStrategy.AllMovementComplete = AllMovementComplete;
        }

        private void AllMovementComplete(IMovable item)
        {
            if (item is IRowEnd r)
            {
                if (!r.RowEnd(out int row, out int column)) return;

                _boardItems[row, column].ReturnToPool();
                ReturnPool(_boardItems[row, column]);

                var voidArea = RetrievePool<VoidArea>(row, column);
                _boardItems[row, column] = voidArea;
                _boardItems[row, column].SetRowAndColumn(row, column);

                RecalculateBoardElements();
                ClearCombineItems();
                ClearRecursiveCheckArray();
            }
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

            ReturnPool(_boardItems[row, column]);

            var randomColor = GenerateRandomColor();
            var item = RetrievePool<Bead>(row, column, randomColor);

            var bead = (Bead)item;
            _boardItems[row, column] = bead;

            bead.RetrieveFromPool();
            bead.SetRowAndColumn(row, column);
            bead.SetSortingOrder(item.GetType().FullName, row, column);
            bead.Color = randomColor;
            bead.MovementStrategy.ResetAllStates();
            bead.SetActive(true);

            AdjustItemPosition(column, row, distanceToNextBead, ref verticalOffset, bead);
            _movementController.Register(bead, bead.MovementStrategy.StartMovement);
            bead.MovementStrategy.AllMovementComplete = AllMovementComplete;
        }


        private void AdjustItemPosition(int column, int row, int distanceToNextBead,
            ref float verticalOffset, Bead bead)
        {
            var offsetRow = 0;
            for (int i = row - 1; i >= 0; i--)
            {
                if (_boardItems[row, column] is not IMovable moveable) continue;
                if (!moveable.IsMoving) continue;

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
                _recursiveCheckArray[row, column] || _boardItems[row, column] is not IMovable moveableItem ||
                moveableItem.IsMoving)
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

        // Helpers
        private void RemoveIfInFinishState(IBoardItem item)
        {
            if (_boardItems[item.Row, item.Column] is IMovable iMoveableItem)
            {
                _movementController.RemoveIfInFinishState(iMoveableItem);
            }
        }

        private void ReplaceWithVoidArea(IBoardItem item)
        {
            ReturnPool(item);
            var voidArea = RetrievePool<VoidArea>(item.Row, item.Column);

            _boardItems[item.Row, item.Column] = voidArea;
            _boardItems[item.Row, item.Column].SetRowAndColumn(item.Row, item.Column);
        }

        private IBoardItem RetrievePool<T>(int row, int column) where T : IBoardItem
        {
            if (!BoardItemPool.Instance.TryRetrieveWithoutParams<T>(out var voidArea))
            {
                voidArea = BoardItemPool.Instance.Retrieve<T>(row, column);
            }

            return voidArea;
        }

        private IBoardItem RetrievePool<T>(int row, int column, object param1) where T : IBoardItem
        {
            if (!BoardItemPool.Instance.TryRetrieveWithoutParams<T>(out var voidArea))
            {
                voidArea = BoardItemPool.Instance.Retrieve<T>(row, column, param1);
            }

            return voidArea;
        }

        private void ReturnPool(IBoardItem item)
        {
            var garbage = _boardItems[item.Row, item.Column];
            BoardItemPool.Instance.Return(garbage);
        }

        private ItemColors GenerateRandomColor()
        {
            return (ItemColors)Random.Range(0, Enum.GetValues(typeof(ItemColors)).Length - 1);
        }

        private void ClearCombineItems()
        {
            _combineItems.Clear();
        }

        private void ClearRecursiveCheckArray()
        {
            Array.Clear(_recursiveCheckArray, 0, _recursiveCheckArray.Length);
        }
    }
}