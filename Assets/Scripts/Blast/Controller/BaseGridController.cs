using System.Collections;
using System.Collections.Generic;
using Blast.View;
using Zenject;
using UnityEngine;

namespace Blast.Controller
{
    public interface IGridController
    {
        public Vector3 CellToLocal(int row, int column);
        public T GetGridView<T>() where T : IGridView;
        
        public Vector3 GetCellSize();
    }

    public abstract class BaseGridController : IGridController
    {
        protected readonly IGridView _gridView;

        protected BaseGridController( IGridView gridView)
        {
            _gridView = gridView;
        }

        public Vector3 CellToLocal(int row, int column)
        {
            return _gridView.Grid.CellToLocal(new Vector3Int(row, column, 0));
        }

        public T GetGridView<T>() where T : IGridView
        {
            return (T)_gridView;
        }

        public Vector3 GetCellSize()
        {
            return _gridView.Grid.cellSize;
        }
    }
}
