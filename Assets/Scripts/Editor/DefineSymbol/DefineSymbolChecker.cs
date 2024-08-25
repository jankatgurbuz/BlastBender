using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Editor.DefineSymbol
{
    [InitializeOnLoad]
    public class DefineSymbolChecker
    {
        private const string Path = "Assets/ScriptableObject/Editor/DefineSymbols.asset";

        static DefineSymbolChecker()
        {
            CheckAndAddDefineSymbols();
        }

        private static void CheckAndAddDefineSymbols()
        {
            var defineSymbols = LoadDefineSymbols();

            if (defineSymbols == null)
            {
                Debug.LogWarning($"DefineSymbols ScriptableObject not found at '{Path}'." +
                                 $" Please create it and add your define symbols.");
                return;
            }

            var existingDefineSymbols = GetExistingDefineSymbols(EditorUserBuildSettings.selectedBuildTargetGroup).ToList();
            var check = false;

            foreach (var item in defineSymbols.Symbols.Where(item => !existingDefineSymbols.Contains(item)))
            {
                Debug.Log($"Define symbol '{item}' added.");
                existingDefineSymbols.Add(item);
                check = true;
            }
        
            if (check)
            {
                PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup,
                    existingDefineSymbols.ToArray());
            }
        }

        private static DefineSymbols LoadDefineSymbols()
        {
            return AssetDatabase.LoadAssetAtPath<DefineSymbols>(Path);
        }

        private static string[] GetExistingDefineSymbols(BuildTargetGroup buildTargetGroup)
        {
            PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup, out string[] defines);
            return defines;
        }
    }
}