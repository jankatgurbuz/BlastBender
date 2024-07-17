using System.Collections;
using System.Collections.Generic;
using Blast.Controller;
using Blast.View;
using Cysharp.Threading.Tasks;
using LevelGenerator.GridSystem.View;
using UnityEngine;
using Zenject;

namespace LevelGenerator.Controller
{
    public interface ILGGridController : ILGStart
    {
        public void CreateIndicators();
        public Vector3 CellToLocal(int row, int column);
    }

    public class LGGridController : BaseGridController, ILGGridController
    {
        private ILevelGeneratorController _levelGeneratorController;
        private ILGGridInteractionController _gridInteractionController;
        private GridIndicator[,] _gridIndicator;
        private DiContainer _container;

        private const int _lgStartRow = -25;
        private const int _lgStartColumn = -25;
        private const int _lgFinishRow = 25;
        private const int _lgFinishColumn = 25;
        private int _rowLength;
        private int _columnLength;

        public LGGridController(DiContainer container, IGridView gridView) : base(gridView)
        {
            _container = container;
        }

        public void Start()
        {
            _levelGeneratorController = _container.Resolve<ILevelGeneratorController>();
            _gridInteractionController = _container.Resolve<ILGGridInteractionController>();

            _gridView.SetPosition();
            _rowLength = Mathf.Abs(_lgStartRow) + Mathf.Abs(_lgFinishRow);
            _columnLength = Mathf.Abs(-_lgStartColumn) + Mathf.Abs(_lgFinishColumn);
            _gridIndicator = new GridIndicator[_rowLength, _columnLength];
            CreateIndicators();

            _gridInteractionController.NoneTouch += NoneTouch;
        }

        private void NoneTouch(int indicatorRow, int indicatorColumn)
        {
            // if (row < 0 || column < 0 || row >= _gridIndicator.GetLength(0) || column >= _gridIndicator.GetLength(1))
            // {
            //     return;
            // }

            for (int row = 0; row < _rowLength; row++)
            {
                for (int column = 0; column < _columnLength; column++)
                {
                    if (row + _lgStartRow == indicatorRow && column + _lgStartColumn == indicatorColumn)
                    {
                        _gridIndicator[row, column].SetColor(Color.red);
                    }
                    else
                    {
                        _gridIndicator[row, column].SetColor(Color.white);
                    }
                }
            }
        }

        public void CreateIndicators()
        {
            Destroy();
            var gridview = GetGridView<LGGridView>();

            for (int row = 0; row < _rowLength; row++)
            {
                for (int column = 0; column < _columnLength; column++)
                {
                    _gridIndicator[row, column] = Object.Instantiate(gridview.GridIndicatorPrefab);
                    _gridIndicator[row, column].transform.SetParent(gridview.transform);
                    _gridIndicator[row, column].SetPosition(CellToLocal(row + _lgStartRow, column + _lgStartColumn));

                    if (!(row + _lgStartRow >= 0 && row + _lgStartRow < _levelGeneratorController.RowLength &&
                          column + _lgStartColumn >= 0 &&
                          column + _lgStartColumn < _levelGeneratorController.ColumnLength))
                    {
                        _gridIndicator[row, column].transform.localScale *= 0.5f;
                    }
                }
            }
        }

        private void Destroy()
        {
            for (int row = 0; row < _rowLength; row++)
            {
                for (int column = 0; column < _columnLength; column++)
                {
                    if (_gridIndicator[row, column] == null)
                        continue;

                    Object.Destroy(_gridIndicator[row, column].gameObject);
                }
            }
        }
    }
}