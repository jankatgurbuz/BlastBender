using Blast.Factory;
using Blast.Installer;
using Gameplay.Pool.BoardItemPool;
using Gameplay.Pool.VoidItem;

namespace BoardItems.Void
{
    public class VoidArea : BaseBoardItem<VoidPoolView>
    {
        public VoidArea(int row, int column) : base(row, column)
        {
            IsVoidArea = true;
        }

        public override IBoardItem Copy(BoardItemFactory factory)
        {
            return factory.Create(GetType(), new object[] { Row, Column });
        }
    }
}