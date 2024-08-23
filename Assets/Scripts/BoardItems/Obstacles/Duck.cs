using UnityEngine;
using Util.Movement.Strategies;
using Util.Pool.BoardItemPool;
using Util.Pool.Duck;

namespace BoardItems.Obstacles
{
    public class Duck : BaseBoardItem<DuckView>, IVisual, IMoveable
    {
        public ItemColors Color { get; set; }
        public IMovementStrategy MovementStrategy { get; set; }

        public Duck(int row, int column) : base(row, column)
        {
            MovementStrategy = new BaseMovementStrategy();
        }

        public override IBoardItem Copy()
        {
            return BoardItemPool.Instance.Retrieve<Duck>(Row, Column);
        }

        protected override void HandleItemActivation(bool isActive)
        {
        }


        public void SetColorAndAddSprite(ItemColors color)
        {
            
        }

        public void SetSortingOrder(int row, int column)
        {
            if (Item != null)
            {
                Item.SetSortingOrder(row, column);
            }
        }
    }
}