using UnityEngine;
using Util.Handlers.Strategies;
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
        void SetPosition(Vector3 position);
        Vector3 GetPosition();
        void SetActive(bool active);
        void SetSortingOrder(int row, int column);
        void Blast();
    }

    public interface IMoveable
    {
        public Transform GetTransform();
        // void StartMovement(IMovementStrategy strategy);
        // void FinalizeMovementWithBounce(IMovementStrategy strategy);
        // void Shake(IMovementStrategy strategy);
    }
}