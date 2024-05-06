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

        public LGGridController(DiContainer container, IGridView gridView) : base(gridView)
        {
            _container = container;
        }

        public void Start()
        {
            _levelGeneratorController = _container.Resolve<ILevelGeneratorController>();
            _gridInteractionController = _container.Resolve<ILGGridInteractionController>();

            _gridView.SetPosition();
            _gridIndicator =
                new GridIndicator[_levelGeneratorController.RowLength, _levelGeneratorController.ColumnLength];
            CreateIndicators();

            _gridInteractionController.NoneTouch += NoneTouch;
        }

        private void NoneTouch(int row, int column)
        {
            if (row < 0 || column < 0 || row >= _gridIndicator.GetLength(0) || column >= _gridIndicator.GetLength(1))
            {
                return;
            }

            for (int i = 0; i < _levelGeneratorController.RowLength; i++)
            {
                for (int j = 0; j < _levelGeneratorController.ColumnLength; j++)
                {
                    if (i == row && j == column)
                    {
                        _gridIndicator[i, j].SetColor(Color.red);
                    }
                    else
                    {
                        _gridIndicator[i, j].SetColor(Color.white);
                    }
                }
            }
        }

        public void CreateIndicators()
        {
            Destroy();
            for (int i = 0; i < _levelGeneratorController.RowLength; i++)
            {
                for (int k = 0; k < _levelGeneratorController.ColumnLength; k++)
                {
                    var gridview = GetGridView<LGGridView>();
                    _gridIndicator[i, k] = Object.Instantiate(gridview.GridIndicatorPrefab);
                    _gridIndicator[i, k].transform.SetParent(gridview.transform);
                    _gridIndicator[i, k].SetPosition(CellToLocal(i, k));
                }
            }
        }

        private void Destroy()
        {
            for (int i = 0; i < _gridIndicator.GetLength(0); i++)
            {
                for (int j = 0; j < _gridIndicator.GetLength(1); j++)
                {
                    if (_gridIndicator[i, j] == null)
                        continue;

                    Object.Destroy(_gridIndicator[i, j].gameObject);
                }
            }

            _gridIndicator =
                new GridIndicator[_levelGeneratorController.RowLength, _levelGeneratorController.ColumnLength];
        }
    }
}