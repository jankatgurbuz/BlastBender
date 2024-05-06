using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EditorScripts.Editor
{
    public class LoadSceneOnPlay : EditorWindow
    {
        private const string _saveKey = "Editor-LoadSceneOnPlay";

        [MenuItem("BlastBender/LoadSceneOnPlay")]
        static void Init()
        {
            var window = GetWindow(typeof(LoadSceneOnPlay));
            window.Show();
        }

        private void OnGUI()
        {
            GUILayout.Label("Select Scene to Load:");

            var scenes = EditorBuildSettings.scenes;
            var sceneNames = new string[scenes.Length];

            for (int i = 0; i < scenes.Length; i++)
            {
                sceneNames[i] = Path.GetFileNameWithoutExtension(scenes[i].path);
            }

            var selectedSceneIndex = EditorGUILayout.Popup(GetSave(), sceneNames);

            PlayerPrefs.SetInt(_saveKey, selectedSceneIndex);
            PlayerPrefs.Save();
        }

        private static int GetSave()
        {
            return PlayerPrefs.GetInt(_saveKey, 0);
        }
        
        [InitializeOnLoad]
        public class PlayFromScene
        {
            static PlayFromScene()
            {
                EditorApplication.playModeStateChanged += LoadMyScene;
            }

            private static void LoadMyScene(PlayModeStateChange state)
            {
                if (state == PlayModeStateChange.ExitingEditMode)
                {
                    var scenePath = SceneUtility.GetScenePathByBuildIndex(GetSave());
                    var scene = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);
                    EditorSceneManager.playModeStartScene = scene;
                }
            }
        }
    }
}
