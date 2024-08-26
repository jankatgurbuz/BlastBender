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

        public override IBoardItem Copy()
        {
            return BoardItemPool.Instance.Retrieve<SpaceArea>(Row, Column);
        }
    }
}