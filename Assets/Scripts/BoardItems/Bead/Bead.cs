using UnityEngine;
using Util.Movement.Strategies;
using Util.Pool.Bead;
using Util.Pool.BoardItemPool;

namespace BoardItems.Bead
{
    public class Bead : BaseBoardItem<BeadView>, IColorable, IVisual, IMoveable
    {
        [SerializeField] private ItemColors _color;
        public IMovementStrategy MovementStrategy { get; set; }
        public bool IsMove { get; set; }

        public ItemColors Color
        {
            get => _color;
            set
            {
                _color = value;
                SetBeadColor();
            }
        }

        public Bead(int row, int column, ItemColors color) : base(row, column)
        {
            Color = color;
            IsBead = true;
            MovementStrategy = new BaseMovementStrategy();
        }

        public override void RetrieveFromPool()
        {
            base.RetrieveFromPool();
            SetBeadColor();
        }


        public override IBoardItem Copy()
        {
            return BoardItemPool.Instance.Retrieve<Bead>(Row, Column, _color);
        }
        //
        // public void SetLayer(int row, int columnOffset)
        // {
        //     if (Item != null)
        //     {
        //         Item.SetLayer(row, columnOffset);
        //     }
        // }

        public void SetSortingOrder(string layerKey,int row, int column)
        {
            Item?.SetSortingOrder(layerKey,row, column);
        }

        private void SetBeadColor()
        {
            if (Item != null)
            {
                Item.Color = _color;
            }
        }
    }
}