using Util.Pool.BoardItemPool;
using Util.Pool.VoidItem;

namespace BoardItems.Void
{
    public class VoidArea : BaseBoardItem<VoidPoolView>
    {
        public VoidArea(int row, int column) : base(row, column)
        {
            IsVoidArea = true;
        }

        public override IBoardItem Copy()
        {
            return BoardItemPool.Instance.Retrieve<VoidArea>(Row, Column);
        }
    }
}