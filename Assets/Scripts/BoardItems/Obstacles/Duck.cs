using Blast.Controller;
using Blast.Factory;
using Gameplay.Movement.Strategies;
using Gameplay.Pool.Duck;
using Zenject;

namespace BoardItems.Obstacles
{
    public class Duck : BaseBoardItem<DuckView>, ISortingOrder, IMovable, IRowEnd
    {
        private BoardItemController _boardItemController;
        public IMovementStrategy MovementStrategy { get; set; }
        public bool IsMoving { get; set; }

        public Duck(int row, int column, IMovementStrategy movementStrategy,
            [Inject(Optional = true)] BoardItemController boardItemController) : base(row, column)
        {
            MovementStrategy = movementStrategy;
            _boardItemController = boardItemController;
        }

        public override IBoardItem Copy(BoardItemFactory factory)
        {
            return factory.Create(GetType(), new object[] { Row, Column });
        }

        public bool RowEnd(out int row, out int column)
        {
            var boardItems = _boardItemController.BoardItems;
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