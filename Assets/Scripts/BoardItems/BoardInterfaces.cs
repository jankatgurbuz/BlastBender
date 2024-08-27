using Blast.Controller;
using Blast.Factory;
using Blast.Installer;
using BoardItems.Util;
using Gameplay.Movement.Strategies;
using Gameplay.Pool;

namespace BoardItems
{
    public interface IBoardItem<TPoolItem> : IBoardItem where TPoolItem : IPoolable
    {
        public TPoolItem Item { get; set; }
    }

    public interface IBoardItem : IItemUtility
    {
        int Row { get; }
        int Column { get; }

        bool IsBead { get; set; }
        bool IsSpace { get; set; }
        bool IsVoidArea { get; set; }
         bool IsRetrievedItem { get; set; }
        void RetrieveFromPool();
        void ReturnToPool();

        void SetRowAndColumn(int row, int column);
        IBoardItem Copy(BoardItemFactory factory);
    }

    public interface IItemUtility
    {
        TransformUtilities TransformUtilities { get; }
        void SetActive(bool active);
    }

    public interface IMovable
    {
        IMovementStrategy MovementStrategy { get; set; }
        TransformUtilities TransformUtilities { get; set; }
        int Row { get; }
        int Column { get; }
        bool IsMoving { get; set; }
    }

    public interface ISortingOrder
    {
        void SetSortingOrder(string layerKey, int row, int column);
    }

    public interface IColorable
    {
        public ItemColors Color { get; set; }
    }

    public interface IRowEnd
    {
        bool RowEnd(out int row, out int column);
    }
}