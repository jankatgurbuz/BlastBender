using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Blast.Controller;
using BoardItems.Border;
using BoardItems.LevelData;
using BoardItems.Void;
using Signals;
using UnityEngine;
using Zenject;

namespace LevelGenerator.Controller
{
    public class LGBorderController : BaseBorderController, ILGStart
    {
        private ILevelGeneratorController _levelGeneratorController;

        public LGBorderController(SignalBus signalBus, BorderProperties borderProperties,
            LGGridController gridController, ILevelGeneratorController levelGeneratorController) : base(
            borderProperties, gridController)
        {
            _levelGeneratorController = levelGeneratorController;
        }

        public void Start()
        {
            _levelGeneratorController.OnChangeState += CreateBorderMatrix;
        }

        private void CreateBorderMatrix()
        {
            var rowLength = _levelGeneratorController.RowLength;
            var columnLength = _levelGeneratorController.ColumnLength;
            var levelData = _levelGeneratorController.LevelData;

            var items = levelData.BoardItem.ToList();

            int margin = 1;
            int expandedRowLength = rowLength + 2 * margin;
            int expandedColumnLength = columnLength + 2 * margin;

            Border[,] boardItems = new Border[expandedRowLength, expandedColumnLength];

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
                static string GetKeyFragment(bool condition) => condition ? "t" : "f";

                int left = item.Column;
                int bottom = item.Row;
                int row = item.Row + 1;
                int column = item.Column + 1;

                if (left < 0 || bottom < 0)
                    continue;

                // corner
                string cornerKey = GetKeyFragment(!item.IsEmpty);
                cornerKey += GetKeyFragment(!boardItems[row, left].IsEmpty);
                cornerKey += GetKeyFragment(!boardItems[bottom, left].IsEmpty);
                cornerKey += GetKeyFragment(!boardItems[bottom, column].IsEmpty);
                item.CornerKey = cornerKey;

                // left-right
                string leftRightKey = GetKeyFragment(!item.IsEmpty);
                leftRightKey += GetKeyFragment(!boardItems[row, left].IsEmpty);
                item.LeftRightKey = leftRightKey;

                // top-bottom
                string topBottomKey = GetKeyFragment(!item.IsEmpty);
                topBottomKey += GetKeyFragment(!boardItems[bottom, column].IsEmpty);
                item.TopBottomKey = topBottomKey;
            }

            levelData.Border = boardItems.Cast<Border>().ToArray();
            
            CreateBorder(levelData.Border);
        }
    }
}