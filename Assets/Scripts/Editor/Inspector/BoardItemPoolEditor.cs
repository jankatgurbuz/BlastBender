using System;
using System.Collections.Generic;
using System.Reflection;
using BoardItems;
using Gameplay.Pool.BoardItemPool;
using UnityEditor;
using UnityEngine;

namespace Editor.Inspector
{
    [CustomEditor(typeof(BoardItemPool))]
    public class BoardItemPoolEditor : UnityEditor.Editor
    {
        private const BindingFlags Flags = BindingFlags.NonPublic | BindingFlags.Instance;

        private void OnEnable()
        {
            EditorApplication.update += UpdateInspector;
        }

        private void OnDisable()
        {
            EditorApplication.update -= UpdateInspector;
        }

        private void UpdateInspector()
        {
            if (!Application.isPlaying || target == null) return;

            Repaint();
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var pool = (BoardItemPool)target;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Board Item Pool Info", EditorStyles.boldLabel);

            var boardItemsMapField = typeof(BoardItemPool).GetField("_boardItemsMap", Flags);
            if (boardItemsMapField == null) return;

            var boardItemsMap = (Dictionary<Type, BoardItemPoolEntry>)boardItemsMapField.GetValue(pool);

            foreach (var item in boardItemsMap)
            {
                EditorGUILayout.LabelField(item.Key.Name, EditorStyles.boldLabel);

                var activeListField = typeof(BoardItemPoolEntry).GetField("_activeList", Flags);
                var inactiveListField = typeof(BoardItemPoolEntry).GetField("_inactiveList", Flags);

                if (activeListField == null || inactiveListField == null) return;

                var activeList = (List<IBoardItem>)activeListField.GetValue(item.Value);
                var inactiveList = (List<IBoardItem>)inactiveListField.GetValue(item.Value);

                EditorGUILayout.LabelField("Active Items: " + activeList.Count);
                EditorGUILayout.LabelField("Inactive Items: " + inactiveList.Count);
              
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            }

            var pendingListField = typeof(BoardItemPool).GetField("_pendingList", Flags);
            if (pendingListField == null) return;

            var pendingList = (List<IBoardItem>)pendingListField.GetValue(pool);

            EditorGUILayout.LabelField("Pending Items: " + pendingList.Count);
        }
    }
}
