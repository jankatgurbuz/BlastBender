using BoardItems.Util;
using UnityEngine;
using Util.Pool;
using Util.Pool.CentralPoolHub;

namespace BoardItems
{
    public abstract class BaseBoardItem<TPoolItem> : IBoardItem<TPoolItem> where TPoolItem : IPoolable, IItemBehavior
    {
        [SerializeField] private int _row;
        [SerializeField] private int _column;
        public TPoolItem Item { get; set; }
        public int Row => _row;
        public int Column => _column;
        public bool IsBead { get; set; }
        public bool IsRetrievedItem { get; set; }
        public bool IsSpace { get; set; }
        public bool IsVoidArea { get; set; }
        public bool IsMove { get; set; }
        // public virtual MovementVisitor MovementVisitor { get; set; } = MovementVisitor.Empty;
        public TransformUtilities TransformUtilities { get; set; }
        public abstract IBoardItem Copy();
        protected abstract void HandleItemActivation(bool isActive);

        protected BaseBoardItem(int row, int column)
        {
            _row = row;
            _column = column;
        }

        public void RetrieveFromPool()
        {
            Item = PoolFactory.Instance.RetrieveFromPool<TPoolItem>();
            TransformUtilities = Item.TransformUtilities;
            HandleItemActivation(true);
            IsRetrievedItem = true;
        }

        public void ReturnToPool()
        {
            if (Item == null)
                return;

            PoolFactory.Instance.ReturnToPool(Item);
            TransformUtilities = null;
            HandleItemActivation(false);
            IsRetrievedItem = false;
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
    }
}