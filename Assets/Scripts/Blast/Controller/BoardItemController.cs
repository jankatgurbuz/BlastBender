using System;
using System.Collections.Generic;
using BoardItems;
using BoardItems.Void;
using Cysharp.Threading.Tasks;
using Global.Controller;
using Signals;
using UnityEngine;
using Zenject;

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
            _combineItems = new List<IBoardItem>();

            foreach (var item in levelData.BoardItem)
            {
                var temp = _boardItems[item.Row, item.Column] = item.Copy();
                temp.RetrieveFromPool();
                temp.BoardVisitor?.Bead?.SetColorAndAddSprite();
                temp.SetSortingOrder(item.Row, item.Column);
                temp.SetPosition(_gridController.CellToLocal(item.Row, item.Column));
                temp.SetActive(true);
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

            var color = item.BoardVisitor.Bead.Color;
            FindMatches(row, column, color);

            switch (_combineItems.Count)
            {
                case > 4: // todo: magic number !!
                    BlastForPowerUp(row, column);
                    break;
                case > 1: // todo: magic number !!
                    Blast();
                    break;
                default:
                    Shake();
                    break;
            }
        }

        private async void BlastForPowerUp(int clickRow, int clickColumn)
        {
            var tempGroup = new List<IBoardItem>(_combineItems);
            _combineItems.Clear();
            Array.Clear(_recursiveCheckArray, 0, _recursiveCheckArray.Length);
            FillVoidType(tempGroup);
            var task = tempGroup.Select(
                item => item.BoardVisitor?.Bead?.CombineBead
                        (item.Row, item.Column, clickRow - item.Row,
                            clickColumn - item.Column)
                        ?? UniTask.CompletedTask);

            await UniTask.WhenAll(task);
            tempGroup.ForEach(item => item.ReturnToPool());
            RecalculateBoardElements();
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

        private void Shake()
        {
            _combineItems[0].MovementVisitor.Shake();
            _combineItems.Clear();
            Array.Clear(_recursiveCheckArray, 0, _recursiveCheckArray.Length);
        }
        private void FillVoidType(List<IBoardItem> combineGroup)
        {
            foreach (var item in combineGroup)
            {
                _boardItems[item.Row, item.Column] = new VoidArea(item.Row, item.Column);
            }
        }

        private void RecalculateBoardElements()
        {
            for (int row = 0; row < _rowLength; row++)
            {
                for (int column = 0; column < _columnLength; column++)
                {
                    ShiftBeadDown(_boardItems[row, column]);
                }
            }

            void ShiftBeadDown(IBoardItem boardItem)
            {
                if (!boardItem.IsVoidArea)
                    return;

                var column = boardItem.Column;
                var row = boardItem.Row;

                if (row >= _rowLength)
                {
                    // todo: Beads falling above
                }

                for (int i = row + 1; i < _rowLength; i++)
                {
                    if (_boardItems[i, column].IsVoidArea || _boardItems[i, column].IsSpace)
                        continue;
                    
                    //swap
                    var item = _boardItems[row, column] = _boardItems[i, column];
                    item.SetRowAndColumn(row, column);
                    item.IsMove = true;
                    _movementController.Register(item, i, column);
                    _boardItems[i, column] = new VoidArea(i, column);
                    break;
                }
            }
        }

        private void FindMatches(int row, int column, ItemColors color)
        {
            if (row < 0 || column < 0 || row >= _rowLength || column >= _columnLength ||
                _recursiveCheckArray[row, column] || _boardItems[row, column].IsMove)
                return;

            _recursiveCheckArray[row, column] = true;
            var match = _boardItems[row, column].IsBead && _boardItems[row, column].BoardVisitor.Bead.Color == color;
            if (!match)
                return;

            _combineItems.Add(_boardItems[row, column]);

            FindMatches(row + 1, column, color);
            FindMatches(row - 1, column, color);
            FindMatches(row, column + 1, color);
            FindMatches(row, column - 1, color);
        }
    }
}