using System;
using System.Reflection;
using Cysharp.Threading.Tasks;
using Global.Controller;
using UnityEditor;
using UnityEngine;
using UnityToolbarExtender;
using Zenject;

namespace EditorScripts.Editor
{
    [InitializeOnLoad]
    public class EditorToolbarIntegrator
    {
        private static double _lastTimeChecked = 0.0d;
        private static readonly float _checkInterval = 1;

        private const string _saveKey = "Editor-EditorToolbarIntegrator";
        private static GUISkin _toolbarSkin;

        static EditorToolbarIntegrator()
        {
            LoadGUISkin();
            ToolbarExtender.LeftToolbarGUI.Add(OnToolbarGUI);
            EditorApplication.playModeStateChanged += HandleOnPlayModeChanged;
            EditorApplication.update += OnUpdate;
        }

        private static void LoadGUISkin()
        {
            _toolbarSkin = AssetDatabase.LoadAssetAtPath<GUISkin>(
                "Assets/Scripts/EditorScripts/ToolBarSkin.guiskin");
            if (_toolbarSkin == null)
                Debug.LogError("Failed to load GUISkin!");
        }

        private static void OnToolbarGUI()
        {
            GUILayout.FlexibleSpace();
            var style = new GUIStyle(_toolbarSkin.button);
            var cont = new GUIContent("Play Level Generator", "Start Level Generator Scene");

            if (GUILayout.Button(cont, style))
            {
                if (CheckGameViewOrientation())
                {
                    ShowExitPlayModeDialog();
                }
                else
                {
                    EnablePlayMode();
                }
            }
        }

        private static void HandleOnPlayModeChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.EnteredPlayMode && GetSave())
            {
                AddLevelGenerator();
            }
        }

        private static async void AddLevelGenerator()
        {
            IGameController gameController;
            do
            {
                await UniTask.Yield();
                gameController = ProjectContext.Instance.Container.TryResolve<IGameController>();
            } while (gameController == null);

            await UniTask.WaitUntil(() => gameController.InitializationComplete);
            await gameController.LevelGeneratorInitialize();
        }

        private static void ShowExitPlayModeDialog()
        {
            var response = EditorUtility.DisplayDialog(
                "Change Game View Orientation",
                "The game screen is currently set to portrait mode. " +
                "You must switch to landscape mode to continue.",
                "Switch to Landscape",
                "Cancel"
            );

            if (response)
            {
                EditorLayoutLoader.LoadLayout(EditorLayoutLoader.LayoutType.Landscape);
                EnablePlayMode();
            }
            else
            {
                DisablePlayMode();
            }
        }

        private static void SetSave(bool active)
        {
            PlayerPrefs.SetInt(_saveKey, active ? 1 : 0);
        }

        private static bool GetSave()
        {
            return PlayerPrefs.GetInt(_saveKey, 0) == 1;
        }

        private static void EnablePlayMode()
        {
            SetSave(true);
            EditorApplication.isPlaying = true;
        }

        private static void DisablePlayMode()
        {
            SetSave(false);
            EditorApplication.isPlaying = false;
        }

        private static bool CheckGameViewOrientation()
        {
            var editorType = Type.GetType("UnityEditor.GameView,UnityEditor");
            MethodInfo gameviewInfo = editorType.GetMethod("GetSizeOfMainGameView",
                BindingFlags.NonPublic | BindingFlags.Static);
            object res = gameviewInfo.Invoke(null, null);
            var resolution = (Vector2)res;
            return resolution.x < resolution.y;
        }

        private static void OnUpdate()
        {
            if (EditorApplication.timeSinceStartup - _lastTimeChecked > _checkInterval)
            {
                _lastTimeChecked = EditorApplication.timeSinceStartup;
                if (!Application.isPlaying && GetSave())
                {
                    SetSave(false);
                }
            }
        }
    }
}
