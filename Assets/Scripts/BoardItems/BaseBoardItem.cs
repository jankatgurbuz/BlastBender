using Blast.Controller;
using BoardItems.Util;
using Gameplay.Pool;
using Gameplay.Pool.CentralPoolHub;
using UnityEngine;

namespace BoardItems
{
    public abstract class BaseBoardItem<TPoolItem> : IBoardItem<TPoolItem> where TPoolItem : IPoolable, IItemUtility
    {
        [SerializeField] private int _row;
        [SerializeField] private int _column;
        public TPoolItem Item { get; set; }
        public int Row => _row;
        public int Column => _column;
        public bool IsBead { get; set; }
        public bool IsRetrievedItem { get; set; }
        public BoardItemController BoardItemController { get; set; }
        public bool IsSpace { get; set; }
        public bool IsVoidArea { get; set; }
        public TransformUtilities TransformUtilities { get; set; }
        public abstract IBoardItem Copy();

        protected BaseBoardItem(int row, int column)
        {
            _row = row;
            _column = column;
        }

        public virtual void RetrieveFromPool()
        {
            Item = PoolFactory.Instance.RetrieveFromPool<TPoolItem>();
            TransformUtilities = Item.TransformUtilities;
            IsRetrievedItem = true;
        }

        public void ReturnToPool()
        {
            if (Item == null)
                return;

            PoolFactory.Instance.ReturnToPool(Item);
            TransformUtilities = null;
            IsRetrievedItem = false;
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