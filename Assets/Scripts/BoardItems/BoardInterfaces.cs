using BoardItems.Util;
using Util.Handlers.Visitors;
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
        bool IsMove { get; set; }
        public bool IsPool { get; set; }
        bool IsSpace { get; set; }
        bool IsVoidArea { get; set; }
        MovementVisitor MovementVisitor { get; set; }
        void RetrieveFromPool();
        void ReturnToPool();
        void SetRowAndColumn(int row, int column);
        IBoardItem Copy();
        
    }

    public interface IItemBehavior
    {
        TransformUtilities TransformUtilities { get; set; }
        void SetActive(bool active);
        void SetSortingOrder(int row, int column);
        void Blast();
    }

    public interface IMoveable
    {
       
    }
}