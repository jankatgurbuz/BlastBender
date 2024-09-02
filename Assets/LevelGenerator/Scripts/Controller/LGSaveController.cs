using System.IO;
using UnityEditor;

namespace LevelGenerator.Scripts.Controller
{
    public interface ILGSaveController
    {
        public void SaveOnClick();
    }
    public class LGSaveController : ILGSaveController
    {
        private const string _path = "Assets/Levels";
        public ILevelGeneratorController _levelGeneratorController;

        public LGSaveController(ILevelGeneratorController levelGeneratorController)
        {
            _levelGeneratorController = levelGeneratorController;
        }
        public void SaveOnClick()
        {
            CreateMyAsset();
        }

        public void CreateMyAsset()
        {
#if UNITY_EDITOR

            int count = Directory.GetFiles(_path, "*.asset", SearchOption.TopDirectoryOnly).Length;
            string assetPath = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(_path, "Level" + ++count + ".asset"));
            AssetDatabase.CreateAsset(_levelGeneratorController.LevelData, assetPath);
            AssetDatabase.SaveAssets();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = _levelGeneratorController.LevelData;
#endif
        }

    }
}
