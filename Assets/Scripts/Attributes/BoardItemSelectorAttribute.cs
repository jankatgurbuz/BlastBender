using UnityEngine;

namespace Attributes
{
    public class BoardItemSelectorAttribute : PropertyAttribute
    {
        public string BooleanFieldName { get; }

        public BoardItemSelectorAttribute(string booleanFieldName)
        {
            BooleanFieldName = booleanFieldName;
        }
    }
}