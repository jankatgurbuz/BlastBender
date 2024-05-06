using System;
using UnityEngine;
using Util.Handlers.Visitors;
using Util.Pool;
using Util.Pool.CentralPoolHub;

namespace BoardItems
{
    public abstract class BaseBoardItem<TPoolItem> : IBoardItem<TPoolItem> where TPoolItem : IPoolable, IItemBehavior
    {
        [SerializeField] private int _row;
        [SerializeField] private int _column;
        private event Action<bool> ItemLifecycleTransition;
        public TPoolItem Item { get; set; }
        public int Row => _row;
        public int Column => _column;
        public bool IsBead { get; set; }
        public bool IsSpace { get; set; }
        public bool IsVoidArea { get; set; }
        public bool IsMove { get; set; }
        public virtual MovementVisitor MovementVisitor { get; set; } = MovementVisitor.Empty;
        public virtual IBoardItemVisitor BoardVisitor
        {
            get => throw new NotImplementedException("Getter for Visitor is not implemented.");
            set => throw new NotImplementedException("Setter for Visitor is not implemented.");
        }

        protected BaseBoardItem(int row, int column)
        {
            _row = row;
            _column = column;
            ItemLifecycleTransition += OnItemLifecycleTransition;
        }
        public abstract IBoardItem Copy();
        protected abstract void OnItemLifecycleTransition(bool isActive);
       
        public void RetrieveFromPool()
        {
            Item = PoolFactory.Instance.RetrieveFromPool<TPoolItem>();
            ItemLifecycleTransition?.Invoke(true);
        }
        public void ReturnToPool()
        {
            if (Item == null)
                return;

            PoolFactory.Instance.ReturnToPool(Item);
            ItemLifecycleTransition?.Invoke(false);
        }
        public virtual void Blast()
        {
            Item?.Blast();
        }
        public void SetRowAndColumn(int row, int column)
        {
            _row = row;
            _column = column;
        }
        public void SetActive(bool active)
        {
            Item?.SetActive(active);
        }
        public void SetSortingOrder(int row, int column)
        {
            Item?.SetSortingOrder(row, column);
        }
        public void SetPosition(Vector3 position)
        {
            Item?.SetPosition(position);
        }
        public Vector3 GetPosition()
        {
            return Item.GetPosition();
        } 
    }
}
