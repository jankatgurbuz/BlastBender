using Blast.Controller;
using BoardItems.Util;
using Util.Movement.Strategies;
using Util.Pool;

namespace BoardItems
{
    public interface IBoardItem<TPoolItem> : IBoardItem where TPoolItem : IPoolable
    {
        public TPoolItem Item { get; set; }
    }

    public interface IBoardItem : IItemBehavior
    {
        int Row { get; }
        int Column { get; }

        bool IsBead { get; set; }
        bool IsSpace { get; set; }
        bool IsVoidArea { get; set; }
        public bool IsRetrievedItem { get; set; }

        public BoardItemController BoardItemController { get; set; }

        void RetrieveFromPool();
        void ReturnToPool();

        void SetRowAndColumn(int row, int column);
        IBoardItem Copy();
    }

    public interface IItemBehavior
    {
        TransformUtilities TransformUtilities { get; set; }
        void SetActive(bool active);
        void Blast();
    }

    public interface IMoveable
    {
        IMovementStrategy MovementStrategy { get; set; }
        TransformUtilities TransformUtilities { get; set; }
        int Row { get; }
        int Column { get; }
        bool IsMove { get; set; }
    }

    public interface IVisual
    {
        // void SetSortingOrder(LayersProperties.ItemName layerProperty,int row, int column);
        void SetSortingOrder(string layerKey,int row, int column);
    }

    public interface IColorable
    {
        public ItemColors Color { get; set; }
    }

    public interface IRowEnd
    {
        bool RowEnd(out int row,out int column);
    }
}