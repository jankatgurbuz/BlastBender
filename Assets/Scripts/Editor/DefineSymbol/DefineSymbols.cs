using System.Collections.Generic;
using UnityEngine;

namespace Editor.DefineSymbol
{
    [CreateAssetMenu(fileName = "DefineSymbols", menuName = "BlastBender/Editor/DefineSymbols", order = 1)]
    public class DefineSymbols : ScriptableObject
    {
        public List<string> Symbols;
    }
}