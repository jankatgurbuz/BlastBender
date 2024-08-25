using System;
using System.Collections.Generic;
using BoardItems;
using Editor.Utility;
using LevelGenerator.Buttons;
using UnityEditor;

namespace LevelGenerator.Editor
{
    [CustomEditor(typeof(ItemSelectedButton), true)]
    [CanEditMultipleObjects]
    public class ItemSelectedButtonEditor : UnityEditor.Editor
    {
        private List<Type> _types;
        private void OnEnable()
        {
            _types = EditorHelpers.GetAssemblies();
        }

        public override void OnInspectorGUI()
        {
            ItemSelectedButton script = (ItemSelectedButton)target;
            var serializedObject = new SerializedObject(script);
            EditorHelpers.DrawPropertiesAutomatically(serializedObject);

            var typeList = EditorHelpers.FindTypeNamesByInterface<IBoardItem>(_types);
            var typeNames = EditorHelpers.ConvertListToArray(typeList);
            var currentIndex = EditorHelpers.FindIndexInArray(typeNames, script.SelectedType);

            EditorGUI.BeginChangeCheck();
            var selectedIndex = EditorGUILayout.Popup("Selected Type:", currentIndex, typeNames);
            if (EditorGUI.EndChangeCheck())
            {
                script.SelectedType = typeList[selectedIndex];
                EditorUtility.SetDirty(script);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
