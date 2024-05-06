#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

namespace EditorScripts
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
            var filePath = Path.Combine(Application.dataPath, "Editor", "Layouts", fileName);

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
#endif