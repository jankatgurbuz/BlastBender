using UnityEngine;
using Util.Movement.Strategies;
using Util.Pool.Bead;
using Util.Pool.BoardItemPool;

namespace BoardItems.Bead
{
    public class Bead : BaseBoardItem<BeadView>, IVisual, IMoveable
    {
        [SerializeField] private ItemColors _color;
        public IMovementStrategy MovementStrategy { get; set; }


        public ItemColors Color
        {
            get => _color;
            set => _color = value;
        }

        public Bead(int row, int column, ItemColors color) : base(row, column)
        {
            _color = color;
            IsBead = true;

            MovementStrategy = new BaseMovementStrategy();
        }

        protected override void HandleItemActivation(bool isActive)
        {
            if (!isActive) return;
        }

        public override IBoardItem Copy()
        {
            return BoardItemPool.Instance.Retrieve<Bead>(Row, Column, _color);
        }

        public void SetLayer(int row, int columnOffset)
        {
            if (Item != null)
            {
                Item.SetLayer(row, columnOffset);
            }
        }

        public void SetColorAndAddSprite(ItemColors color)
        {
            _color = color;
            Item?.SetColorAndAddSprite(_color);
        }

        public void SetSortingOrder(int row, int column)
        {
            Item?.SetSortingOrder(row, column);
        }
    }
}