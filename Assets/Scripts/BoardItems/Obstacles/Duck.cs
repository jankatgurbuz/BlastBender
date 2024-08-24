using Util.Movement.Strategies;
using Util.Pool.BoardItemPool;
using Util.Pool.Duck;

namespace BoardItems.Obstacles
{
    public class Duck : BaseBoardItem<DuckView>, IVisual, IMoveable, IRowEnd
    {
        public ItemColors Color { get; set; }
        public IMovementStrategy MovementStrategy { get; set; }
        public bool IsMove { get; set; }

        public Duck(int row, int column) : base(row, column)
        {
            MovementStrategy = new BaseMovementStrategy();
        }

        public override IBoardItem Copy()
        {
            return BoardItemPool.Instance.Retrieve<Duck>(Row, Column);
        }


        public void SetColorAndAddSprite(ItemColors color)
        {
        }

        public bool RowEnd(out int row,out int column)
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

        public void SetSortingOrder(int row, int column)
        {
            if (Item != null)
            {
                Item.SetSortingOrder(row, column);
            }
        }
    }
}