using System.Collections.Generic;
using BoardItems.Border;
using BoardItems.LevelData;
using Util.Pool.Border;

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

                ConfigureBorders(_borderProperties.Center, "center", item.Row, item.Column);
            }
        }

        private void ConfigureBorders(List<BorderProperties.BorderInfo> mapData, string key, int row, int column)
        {
            var borderData = mapData.Find(x => x.Key == key);

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
    }
}