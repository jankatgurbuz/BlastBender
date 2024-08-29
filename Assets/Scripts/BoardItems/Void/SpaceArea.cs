using Blast.Factory;
using Blast.Installer;
using Gameplay.Pool.BoardItemPool;
using Gameplay.Pool.VoidItem;

namespace BoardItems.Void
{
    public class SpaceArea : BaseBoardItem<VoidPoolView>
    {
        public SpaceArea(int row, int column) : base(row, column)
        {
            IsSpace = true;
        }

        public override IBoardItem Copy(BoardItemFactory factory)
        {
            return factory.Create(GetType(), new object[] { Row, Column });
        }
    }
}