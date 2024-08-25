using System;
using System.Linq;
using Attributes;
using BoardItems;
using UnityEditor;
using UnityEngine;

namespace EditorScripts.Editor.AttributeEditor
{
    [CustomPropertyDrawer(typeof(BoardItemSelectorAttribute))]
    public class BoardItemSelectorDrawer : PropertyDrawer
    {
        private static readonly string[] _typeNames;

        static BoardItemSelectorDrawer()
        {
            var boardItemTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => typeof(IBoardItem).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
                .ToList();

            _typeNames = boardItemTypes.Select(t => t.FullName).ToArray();
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var dropdownAttribute = (BoardItemSelectorAttribute)attribute;
            var booleanProperty = property.serializedObject.FindProperty(property.propertyPath.Replace(
                property.name, dropdownAttribute.BooleanFieldName));

            if (booleanProperty != null && booleanProperty.boolValue)
            {
                if (_typeNames == null || _typeNames.Length == 0) return;

                var selectedIndex = Array.IndexOf(_typeNames, property.stringValue);
                if (selectedIndex == -1) selectedIndex = 0;

                selectedIndex = EditorGUI.Popup(position, label.text, selectedIndex, _typeNames);
                property.stringValue = _typeNames[selectedIndex];
            }
            else
            {
                property.stringValue = EditorGUI.TextField(position, label, property.stringValue);
            }

            property.serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(property.serializedObject.targetObject);
        }
    }
}