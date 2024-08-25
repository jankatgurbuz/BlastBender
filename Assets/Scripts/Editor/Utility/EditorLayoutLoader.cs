using System.IO;
using UnityEditor;
using UnityEngine;

namespace Editor.Utility
{
    public static class EditorLayoutLoader
    {
        public enum LayoutType
        {
            Portrait,
            Landscape
        }

        public static void LoadLayout(LayoutType layoutType)
        {
            var fileName = layoutType == LayoutType.Portrait ? "Portrait.wlt" : "Landscape.wlt";
            var filePath = Path.Combine(Application.dataPath, "Scripts", "Editor", "Layouts", fileName);

            if (File.Exists(filePath))
            {
                EditorUtility.LoadWindowLayout(filePath);
            }
            else
            {
                Debug.LogError("Layout file not found: " + filePath);
            }
        }
    }
}