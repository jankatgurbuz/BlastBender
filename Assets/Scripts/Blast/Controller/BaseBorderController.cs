using System.Collections.Generic;
using System.Linq;
using BoardItems.Border;
using BoardItems.LevelData;
using BoardItems.Void;
using Gameplay.Pool.Border;

namespace Blast.Controller
{
    public class BaseBorderController
    {
        private readonly BorderProperties _borderProperties;
        private readonly IGridController _gridController;

        protected BaseBorderController(BorderProperties borderProperties, IGridController gridController)
        {
            _gridController = gridController;
            _borderProperties = borderProperties;
        }

        public void CreateBorder(Border[] border)
        {
            Clean();
            foreach (var item in border)
            {
                ConfigureBorders(_borderProperties.CornerBorders, item.CornerKey, item.Row, item.Column);
                ConfigureBorders(_borderProperties.HorizontalBorders, item.LeftRightKey, item.Row, item.Column);
                ConfigureBorders(_borderProperties.VerticalBorders, item.TopBottomKey, item.Row, item.Column);

                if (!item.HasNeighbors || item.IsEmpty)
                    continue;

                ConfigureBorders(_borderProperties.Center, (int)BordersType.Corner_Center, item.Row,
                    item.Column);
            }
        }

        private void ConfigureBorders(List<BorderProperties.BorderInfo> mapData, int key, int row, int column)
        {
            var borderData = mapData.Find(x => (int)x.Key == key);

            if (borderData == null)
                return;

            Create(row, column, borderData);
        }

        private void Create(int row, int column, BorderProperties.BorderInfo borderData)
        {
            var borderItem = BorderPool.Instance.Retrieve();
            borderItem.SetSprite(borderData.Sprite);

            var vec = _gridController.CellToLocal(row, column);
            borderItem.SetPosition(vec + borderData.Offset);
        }

        public void Clean()
        {
            BorderPool.Instance.Clear();
        }

        protected void CreateBorderMatrix(int rowLength, int columnLength, LevelData levelData)
        {
            var items = levelData.BoardItem.ToList();

            const int margin = 1;
            var expandedRowLength = rowLength + 2 * margin;
            var expandedColumnLength = columnLength + 2 * margin;

            var boardItems = new Border[expandedRowLength, expandedColumnLength];

            for (int i = 0; i < expandedRowLength; i++)
            {
                for (int j = 0; j < expandedColumnLength; j++)
                {
                    var itemRow = i - margin;
                    var itemColumn = j - margin;
                    var item = items.Find(x => x.Row == itemRow && x.Column == itemColumn);
                    var isNeighbor = item != null && itemRow >= 0 && itemColumn >= 0;
                    var isEmpty = item is SpaceArea || item == null;
                    boardItems[i, j] = new Border(isNeighbor, isEmpty, itemRow, itemColumn);
                }
            }

            // arrange Neighbors
            foreach (var item in boardItems)
            {
                var left = item.Column;
                var bottom = item.Row;
                var row = item.Row + 1;
                var column = item.Column + 1;

                if (left < 0 || bottom < 0)
                    continue;

                // corner
                var cornerKey = GetKeyFragment(!item.IsEmpty);
                cornerKey = JoinNumber(cornerKey, GetKeyFragment(!boardItems[row, left].IsEmpty));
                cornerKey = JoinNumber(cornerKey, GetKeyFragment(!boardItems[bottom, left].IsEmpty));
                cornerKey = JoinNumber(cornerKey, GetKeyFragment(!boardItems[bottom, column].IsEmpty));
                item.CornerKey = cornerKey;

                // left-right
                var leftRightKey = GetKeyFragment(!item.IsEmpty);
                leftRightKey = JoinNumber(leftRightKey, GetKeyFragment(!boardItems[row, left].IsEmpty));
                leftRightKey = JoinNumber(leftRightKey, 2);
                item.LeftRightKey = leftRightKey;
                //
                // // top-bottom
                var topBottomKey = GetKeyFragment(!item.IsEmpty);
                topBottomKey = JoinNumber(topBottomKey, GetKeyFragment(!boardItems[bottom, column].IsEmpty));
                topBottomKey = JoinNumber(topBottomKey, 3);
                item.TopBottomKey = topBottomKey;
            }

            CreateBorder(boardItems.Cast<Border>().ToArray());
        }

        private int GetKeyFragment(bool condition) => condition ? 1 : 0;
        private int JoinNumber(int number1, int number2) => number1 * 10 + number2;
    }
}