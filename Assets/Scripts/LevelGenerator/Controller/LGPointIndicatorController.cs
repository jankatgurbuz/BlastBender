using System;
using LevelGenerator.GridSystem.View;
using UnityEngine;
using Object = UnityEngine.Object;

namespace LevelGenerator.Controller
{
    public class LGPointIndicatorController : ILGStart
    {
        private const int _lgStartRow = -25;
        private const int _lgStartColumn = -25;
        private const int _lgFinishRow = 25;
        private const int _lgFinishColumn = 25;

        private GridIndicator[,] _gridIndicator;

        private ILevelGeneratorController _levelGeneratorController;
        private LGGridController _gridController;

        private int _rowLength;
        private int _columnLength;

        public LGPointIndicatorController(ILevelGeneratorController levelGeneratorController,
            ILGGridInteractionController gridInteractionController, LGGridController gridController)
        {
            _levelGeneratorController = levelGeneratorController;
            _gridController = gridController;

            gridInteractionController.NoneTouch += NoneTouch;
        }

        public void Start()
        {
            Initialize();
            _levelGeneratorController.OnChangeState += CreateIndicators;
        }

        private void Initialize()
        {
            _rowLength = Mathf.Abs(_lgStartRow) + Mathf.Abs(_lgFinishRow);
            _columnLength = Mathf.Abs(-_lgStartColumn) + Mathf.Abs(_lgFinishColumn);
            _gridIndicator = new GridIndicator[_rowLength, _columnLength];
        }

        private void CreateIndicators()
        {
            Destroy();
            var gridview = _gridController.GetGridView<LGGridView>();

            IteratePoints((row, column) =>
            {
                _gridIndicator[row, column] = Object.Instantiate(gridview.GridIndicatorPrefab);
                _gridIndicator[row, column].transform.SetParent(gridview.transform);
                _gridIndicator[row, column]
                    .SetPosition(_gridController.CellToLocal(row + _lgStartRow, column + _lgStartColumn));

                if (!(row + _lgStartRow >= 0 && row + _lgStartRow < _levelGeneratorController.RowLength &&
                      column + _lgStartColumn >= 0 &&
                      column + _lgStartColumn < _levelGeneratorController.ColumnLength))
                {
                    _gridIndicator[row, column].transform.localScale *= 0.5f;
                }
            });
        }

        private void Destroy()
        {
            IteratePoints((row, column) =>
            {
                if (_gridIndicator[row, column] == null) return;

                Object.Destroy(_gridIndicator[row, column].gameObject);
            });
        }

        private void NoneTouch(int indicatorRow, int indicatorColumn)
        {
            IteratePoints((row, column) =>
            {
                if (row + _lgStartRow == indicatorRow && column + _lgStartColumn == indicatorColumn)
                    _gridIndicator[row, column].SetColor(Color.red);
                else
                    _gridIndicator[row, column].SetColor(Color.white);
            });
        }

        private void IteratePoints(Action<int, int> action)
        {
            for (int row = 0; row < _rowLength; row++)
            for (int column = 0; column < _columnLength; column++)
                action(row, column);
        }
    }
}