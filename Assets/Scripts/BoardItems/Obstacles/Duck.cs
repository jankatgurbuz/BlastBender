using Gameplay.Movement.Strategies;
using Gameplay.Pool.BoardItemPool;
using Gameplay.Pool.Duck;

namespace BoardItems.Obstacles
{
    public class Duck : BaseBoardItem<DuckView>, ISortingOrder, IMovable, IRowEnd
    {
        public IMovementStrategy MovementStrategy { get; set; }
        public bool IsMoving { get; set; }

        public Duck(int row, int column) : base(row, column)
        {
            //todo DuckMovementStrategy
            MovementStrategy = new BeadMovementStrategy();
        }

        public override IBoardItem Copy()
        {
            return BoardItemPool.Instance.Retrieve<Duck>(Row, Column);
        }

        public bool RowEnd(out int row, out int column)
        {
            var boardItems = BoardItemController.BoardItems;
            var check = true;
            for (int i = Row - 1; i >= 0; i--)
            {
                if (boardItems[i, Column].IsSpace) continue;

                check = false;
                break;
            }

            row = Row;
            column = Column;
            return check;
        }

        public void SetSortingOrder(string layerKey, int row, int column)
        {
            if (Item != null)
            {
                Item.SetSortingOrder(layerKey, row, column);
            }
        }
    }
}