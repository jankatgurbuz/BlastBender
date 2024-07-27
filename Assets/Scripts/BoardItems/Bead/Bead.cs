using UnityEngine;
using Util.Handlers.Visitors;
using Util.Movement.Strategies;
using Util.Pool.Bead;

namespace BoardItems.Bead
{
    public class Bead : BaseBoardItem<BeadView>
    {
        [SerializeField] private ItemColors _color;
        public ItemColors Color => _color;

        public Bead(int row, int column, ItemColors color) : base(row, column)
        {
            _color = color;
            IsBead = true;

            MovementVisitor = new MovementVisitor(new BaseMovementStrategy());
        }

        protected override void HandleItemActivation(bool isActive)
        {
            if (!isActive) return;

            MovementVisitor.MoveableItem = Item;
        }

        public override IBoardItem Copy()
        {
            return new Bead(Row, Column, _color);
        }

        public void SetColorAndAddSprite()
        {
            Item?.SetColorAndAddSprite(_color);
        }

        public void SetLayer(int row, int columnOffset)
        {
            if (Item != null)
            {
                Item.SetLayer(row, columnOffset);
            }
        }
    }
}