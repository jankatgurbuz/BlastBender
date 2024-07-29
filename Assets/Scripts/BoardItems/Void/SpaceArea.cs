using Util.Handlers.Visitors;
using Util.Pool.BoardItemPool;
using Util.Pool.NullPtrPool;

namespace BoardItems.Void
{
    public class SpaceArea : BaseBoardItem<VoidPoolView>
    {
        public SpaceArea(int row, int column) : base(row, column)
        {
            IsSpace = true;
        }

        public override IBoardItem Copy()
        {
            return BoardItemPool.Instance.Retrieve<SpaceArea>(Row, Column);
        }

        protected override void HandleItemActivation(bool isActive)
        {
        }
    }
}