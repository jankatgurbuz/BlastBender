using System;
using System.Collections.Generic;
using System.Linq;
using BoardItems;
using BoardItems.LevelData;
using UnityEditor;
using UnityEngine;

namespace Editor.AttributeEditor
{
    [CustomEditor(typeof(LevelData))]
    public class BoardItemDrawer : UnityEditor.Editor
    {
        private Type[] _boardItemTypes;
        private string[] _boardItemTypeNames;

        private void OnEnable()
        {
            _boardItemTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => typeof(IBoardItem).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
                .ToArray();

            _boardItemTypeNames = _boardItemTypes.Select(type => type.Name).ToArray();
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.Space();
            serializedObject.ApplyModifiedProperties();

            var child = true;
            var prop = serializedObject.GetIterator();
            while (prop.NextVisible(child))
            {
                child = false;
                if (prop.name == "BoardItem")
                {
                    DrawBoardItem(prop);
                    continue;
                }

                EditorGUILayout.PropertyField(prop, true);
            }
        }

        private void DrawBoardItem(SerializedProperty serializedProperty)
        {
            serializedProperty.isExpanded = EditorGUILayout.Foldout(serializedProperty.isExpanded, "Board Items", true);
            if (!serializedProperty.isExpanded) return;


            EditorGUI.indentLevel++;

            DrawArraySizeForBoardElement(serializedProperty);

            for (int i = 0; i < serializedProperty.arraySize; i++)
            {
                SerializedProperty item = serializedProperty.GetArrayElementAtIndex(i);
                if (item == null)
                {
                    EditorGUILayout.HelpBox($"Item at index {i} is null.", MessageType.Error);
                    continue;
                }

                item.isExpanded = EditorGUILayout.Foldout(item.isExpanded, $"Element {i}", true);
                if (item.isExpanded)
                {
                    EditorGUI.indentLevel++;

                    var currentTypeIndex = GetIndexNumber(item);
                    var selectedTypeIndex = EditorGUILayout.Popup("Type", currentTypeIndex, _boardItemTypeNames);

                    if (selectedTypeIndex != currentTypeIndex || item.managedReferenceValue == null)
                    {
                        var selectedType = _boardItemTypes[selectedTypeIndex];
                        var allParams = RecalculateParams(selectedType);
                        item.managedReferenceValue = Activator.CreateInstance(selectedType, allParams);
                    }

                    var child = item;
                    var depth = child.depth;
                    child.NextVisible(true);
                    do
                    {
                        if (child.depth == depth + 1)
                        {
                            EditorGUILayout.PropertyField(child, true);
                        }
                    } while (child.NextVisible(false));

                    EditorGUI.indentLevel--;
                }
            }

            EditorGUI.indentLevel--;
        }

        private void DrawArraySizeForBoardElement(SerializedProperty serializedProperty)
        {
            var arraySizeProp = serializedProperty.FindPropertyRelative("Array.size");
            EditorGUILayout.PropertyField(arraySizeProp, new GUIContent("Size"));

            if (serializedProperty.arraySize != arraySizeProp.intValue)
            {
                serializedProperty.arraySize = arraySizeProp.intValue;
            }
        }

        private int GetIndexNumber(SerializedProperty item)
        {
            var currentTypeIndex = Array.FindIndex(_boardItemTypes,
                type => item.managedReferenceFullTypename?.Contains(type.Name) ?? false);

            if (currentTypeIndex < 0)
            {
                currentTypeIndex = Array.FindIndex(_boardItemTypes, x => x.Name.Contains("Bead"));
            }

            return currentTypeIndex;
        }

        private object[] RecalculateParams(Type item)
        {
            var paramList = new List<object>();

            var allConstructors = item.GetConstructors();
            var firstConstructor = allConstructors.First();
            var allParams = firstConstructor.GetParameters();

            foreach (var param in allParams)
            {
                if (param.GetType().IsValueType)
                {
                    paramList.Add(0);
                }
                else
                {
                    paramList.Add(null);
                }
            }

            return paramList.ToArray();
        }
    }
}