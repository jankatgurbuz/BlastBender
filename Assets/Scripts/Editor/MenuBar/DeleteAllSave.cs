using UnityEditor;
using UnityEngine;

namespace Editor.MenuBar
{
    public class DeleteAllSave : EditorWindow
    {
        [MenuItem("BlastBender/DeleteAllSave")]
        static void Init()
        {
            PlayerPrefs.DeleteAll();
            EditorPrefs.DeleteAll();
            PlayerPrefs.Save();
        }
    }
}