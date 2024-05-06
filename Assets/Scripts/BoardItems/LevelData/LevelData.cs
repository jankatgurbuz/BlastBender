using UnityEngine;

namespace BoardItems.LevelData
{
    [CreateAssetMenu(fileName = "Level", menuName = "Level")]
    public class LevelData : ScriptableObject
    {
        public int RowLength;
        public int ColumnLength;
        [SerializeReference] public IBoardItem[] BoardItem;
        public Border.Border[] Border;
    }
}