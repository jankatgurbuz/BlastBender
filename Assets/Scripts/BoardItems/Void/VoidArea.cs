using Util.Handlers.Visitors;
using Util.Pool.NullPtrPool;

namespace BoardItems.Void
{
    public class VoidArea : BaseBoardItem<VoidPoolView>
    {
        public sealed override IBoardItemVisitor BoardVisitor { get; set; }

        public VoidArea(int row, int column) : base(row, column)
        {
            IsVoidArea = true;
            BoardVisitor = new BoardItemVisitor(this);
        }

        protected override void OnItemLifecycleTransition(bool isActive)
        {
        }

        public override IBoardItem Copy()
        {
            return new VoidArea(Row, Column);
        }
    }
}