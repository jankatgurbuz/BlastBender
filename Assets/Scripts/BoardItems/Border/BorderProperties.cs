using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace BoardItems.Border
{
    [CreateAssetMenu(fileName = "NewBorderProperties", menuName = "BlastBender/Game/BorderProperties")]
    public class BorderProperties : ScriptableObject
    {
        public List<BorderInfo> CornerBorders;
        public List<BorderInfo> HorizontalBorders;
        public List<BorderInfo> VerticalBorders;
        public List<BorderInfo> Center;

        [System.Serializable]
        public class BorderInfo
        {
            [HorizontalLine(color: EColor.Orange)]
            public string Key;
            [ShowAssetPreview(32, 32)]
            public Sprite Sprite;
            public Vector3 Offset;
        }
    }
}
