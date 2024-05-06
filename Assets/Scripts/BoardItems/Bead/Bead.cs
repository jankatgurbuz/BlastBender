using Cysharp.Threading.Tasks;
using UnityEngine;
using Util.Handlers.Strategies;
using Util.Handlers.Visitors;
using Util.Pool.Bead;

namespace BoardItems.Bead
{
    public class Bead : BaseBoardItem<BeadView>
    {
        [SerializeField] private ItemColors _color;
        public  ItemColors Color => _color;
        public sealed override IBoardItemVisitor BoardVisitor { get; set; }
        public sealed override MovementVisitor MovementVisitor { get; set; }

        public Bead(int row, int column, ItemColors color) : base(row, column)
        {
            _color = color;
            IsBead = true;
            BoardVisitor = new BoardItemVisitor(this);
        }
        protected override void OnItemLifecycleTransition(bool isActive)
        {
            if (isActive)
            {
                MovementVisitor = new MovementVisitor(Item,new NormalMovementStrategy());
            }
        }
        public override IBoardItem Copy()
        {
            return new Bead(Row, Column, _color);
        }
        
        public void SetColorAndAddSprite()
        {
            Item?.SetColorAndAddSprite(_color);
        }
       
        public UniTask CombineBead(int row, int column, int rowOffset, int columnOffset)
        {
            return Item == null ? UniTask.CompletedTask : Item.CombineBead(row, column, rowOffset, columnOffset);
        }
    }
}
