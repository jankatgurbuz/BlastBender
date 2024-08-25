using System.Collections.Generic;
using Attributes;
using UnityEngine;
using NaughtyAttributes;

namespace Global.View
{
    [CreateAssetMenu(fileName = "LayersProperties", menuName = "BlastBender/Game/LayersProperties", order = 1)]
    public class LayersProperties : ScriptableObject
    {
        [SerializeField] private LayerProperties[] _properties;

        private Dictionary<string, LayerProperties> _layerPropertiesLookup;

        //lazy
        public LayerProperties this[string itemName]
        {
            get
            {
                if (_layerPropertiesLookup == null)
                {
                    _layerPropertiesLookup = new Dictionary<string, LayerProperties>();
                    foreach (LayerProperties property in _properties)
                    {
                        _layerPropertiesLookup.Add(property.Type, property);
                    }
                }
                Debug.Log(itemName);
                return _layerPropertiesLookup[itemName];
            }
        }


        [System.Serializable]
        public class LayerProperties
        {
            public bool IsBoardItemKey = true;
            [BoardItemSelector("IsBoardItemKey")] public string Type;
            [SortingLayer] public int SortingLayer;
            public int OrderInLayer;
        }

        // public enum ItemName
        // {
        //     Bead,
        //     BeadBurstEffect,
        //     BeadBurstParticle,
        //     CombineBeads,
        //     RectangleBeadCombinationEffect
        // }
    }
}