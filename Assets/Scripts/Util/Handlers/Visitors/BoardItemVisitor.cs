using BoardItems.Bead;
using BoardItems.Void;

namespace Util.Handlers.Visitors
{
    public interface IBoardItemVisitor
    {
        Bead Bead { get; }
        VoidArea VoidArea { get; }
        SpaceArea SpaceArea { get; }
    }

    public class BoardItemVisitor :IBoardItemVisitor
    {
        public Bead Bead { get; }
        public VoidArea VoidArea { get; }
        public SpaceArea SpaceArea { get; }

        public BoardItemVisitor(Bead bead)
        {
            Bead = bead;
        }
        public BoardItemVisitor(VoidArea voidArea)
        {
            VoidArea = voidArea;
        }
        public BoardItemVisitor(SpaceArea spaceArea)
        {
            SpaceArea = spaceArea;
        }
    }
}