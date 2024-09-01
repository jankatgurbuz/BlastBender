using System;
using System.Linq;
using System.Reflection;
using Cysharp.Threading.Tasks;
using Global.Controller;
using UnityEditor;
using UnityEngine;
using UnityToolbarExtender;
using Zenject;

namespace Editor.Utility
{
    [InitializeOnLoad]
    public class EditorToolbarIntegrator
    {
        private const string SaveKey = "Editor-EditorToolbarIntegrator";
        private const string ToolBarPath = "Assets/Scripts/Editor/ToolBarSkin.guiskin";
        private const string LevelGeneratorScenePath = "Assets/Scenes/LevelGenerator.unity";

        private static double _lastTimeChecked = 0.0d;
        private static readonly float _checkInterval = 1;

        private static GUIStyle _guiStyle;
        private static GUIContent _guiContent;

        static EditorToolbarIntegrator()
        {
            LoadGUISkin();
            ToolbarExtender.LeftToolbarGUI.Add(OnToolbarGUI);
            EditorApplication.playModeStateChanged += HandleOnPlayModeChanged;
            EditorApplication.update += OnUpdate;
        }

        private static void LoadGUISkin()
        {
            var toolbarSkin = AssetDatabase.LoadAssetAtPath<GUISkin>(ToolBarPath);

            if (toolbarSkin == null)
                Debug.LogError("Failed to load GUISkin!");

            _guiStyle = new GUIStyle(toolbarSkin.button);
            _guiContent = new GUIContent("Play Level Generator", "Start Level Generator Scene");
        }

        private static void OnToolbarGUI()
        {
            GUILayout.FlexibleSpace();


            if (!GUILayout.Button(_guiContent, _guiStyle)) return;

            if (CheckGameViewOrientation())
            {
                ShowExitPlayModeDialog();
            }
            else
            {
                EnablePlayMode();
            }
        }

        private static bool CheckGameViewOrientation()
        {
            var editorType = Type.GetType("UnityEditor.GameView,UnityEditor");

            var gameViewInfo = editorType?.GetMethod("GetSizeOfMainGameView",
                BindingFlags.NonPublic | BindingFlags.Static);

            var r = gameViewInfo?.Invoke(null, null);

            var resolution = (Vector2)r!;
            return resolution.x < resolution.y;
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

        private static void HandleOnPlayModeChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingEditMode && GetSave())
            {
                AddSceneToBuildSettings();
            }

            if (state == PlayModeStateChange.EnteredPlayMode && GetSave())
            {
                AddLevelGenerator();
            }

            if (state == PlayModeStateChange.ExitingPlayMode)
            {
                DeleteLevelGeneratorFromEditorBuildSettings();
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

        private static void AddSceneToBuildSettings()
        {
            if (EditorBuildSettings.scenes.Any(scene => scene.path == LevelGeneratorScenePath)) return;

            var scenes = EditorBuildSettings.scenes.ToList();
            scenes.Add(new EditorBuildSettingsScene(LevelGeneratorScenePath, true));
            EditorBuildSettings.scenes = scenes.ToArray();
        }

        private static void DeleteLevelGeneratorFromEditorBuildSettings()
        {
            var scenes = EditorBuildSettings.scenes.ToList();

            var sceneToRemove = scenes.FirstOrDefault(scene => scene.path == LevelGeneratorScenePath);
            if (sceneToRemove != null)
            {
                scenes.Remove(sceneToRemove);
                EditorBuildSettings.scenes = scenes.ToArray();
            }
        }

        private static void SetSave(bool active)
        {
            PlayerPrefs.SetInt(SaveKey, active ? 1 : 0);
        }

        private static bool GetSave()
        {
            return PlayerPrefs.GetInt(SaveKey, 0) == 1;
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