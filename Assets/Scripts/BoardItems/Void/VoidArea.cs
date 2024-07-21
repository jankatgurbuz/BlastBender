using Util.Handlers.Visitors;
using Util.Pool.NullPtrPool;

namespace BoardItems.Void
{
    public class VoidArea : BaseBoardItem<VoidPoolView>
    {
        public VoidArea(int row, int column) : base(row, column)
        {
            IsVoidArea = true;
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