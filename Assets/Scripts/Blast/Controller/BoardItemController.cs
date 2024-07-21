using System;
using System.Collections.Generic;
using BoardItems;
using BoardItems.Bead;
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
                if (temp is Bead bead)
                {
                    bead.SetColorAndAddSprite();
                }

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

            var color = ((Bead)item).Color;
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

            var tasks = tempGroup.Select(item =>
            {
                if (item is Bead bead)
                {
                    return bead.CombineBead(item.Row, item.Column, clickRow - item.Row, clickColumn - item.Column);
                }

                return UniTask.CompletedTask;
            });

            await UniTask.WhenAll(tasks);
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
                if (boardItem.IsBead)
                    return;

                var column = boardItem.Column;
                var row = boardItem.Row;

                for (int i = row + 1; i <= _rowLength; i++)
                {
                    if (i < _rowLength && _boardItems[i, column].IsBead)
                    {
                        var item = _boardItems[row, column] = _boardItems[i, column];
                        item.SetRowAndColumn(row, column);
                        item.SetSortingOrder(row, column);
                        item.IsMove = true;
                        _movementController.Register(item, i, column);
                        _boardItems[i, column] = new VoidArea(i, column);
                        break;
                    }

                    if (i == _rowLength)
                    {
                        _boardItems[row, column] = new Bead(row, column, ItemColors.Red);
                        // (ItemColors)UnityEngine.Random.Range(1, Enum.GetValues(typeof(ItemColors)).Length) - 1);
                        _boardItems[row, column].RetrieveFromPool();

                        ((Bead)_boardItems[row, column]).SetColorAndAddSprite();
                        _boardItems[row, column].SetSortingOrder(row, column);
                        _boardItems[row, column].SetPosition(_gridController.CellToLocal(row + 10, column));
                        _boardItems[row, column].SetActive(true);
                        _boardItems[row, column].IsMove = true;

                        _movementController.Register(_boardItems[row, column], i, column);
                    }
                    //  (ItemColors)Random.Range(1, Enum.GetValues(typeof(ItemColors)).Length)
                }
            }
        }

        private void FindMatches(int row, int column, ItemColors color)
        {
            if (row < 0 || column < 0 || row >= _rowLength || column >= _columnLength ||
                _recursiveCheckArray[row, column] || _boardItems[row, column].IsMove)
                return;

            _recursiveCheckArray[row, column] = true;
            var match = _boardItems[row, column].IsBead && ((Bead)_boardItems[row, column]).Color == color;
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