using Util.Handlers.Visitors;
using Util.Pool.NullPtrPool;

namespace BoardItems.Void
{
    public class SpaceArea : BaseBoardItem<VoidPoolView>
    {
        public sealed override IBoardItemVisitor BoardVisitor { get; set; }

        public SpaceArea(int row, int column) : base(row, column)
        {
            IsSpace = true;
            BoardVisitor = new BoardItemVisitor(this);
        }

        public override IBoardItem Copy()
        {
            return new SpaceArea(Row, Column);
        }

        protected override void OnItemLifecycleTransition(bool isActive)
        {
        }
    }
}