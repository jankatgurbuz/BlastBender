using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace Global.View
{
    [CreateAssetMenu(fileName = "LayersProperties", menuName = "LayersProperties", order = 1)]
    public class LayersProperties : ScriptableObject
    {
        [SerializeField] private LayerProperties[] _properties;

        private Dictionary<ItemName, LayerProperties> _layerPropertiesLookup;

        //lazy
        public LayerProperties this[ItemName itemName]
        {
            get
            {
                if (_layerPropertiesLookup == null)
                {
                    _layerPropertiesLookup = new Dictionary<ItemName, LayerProperties>();
                    foreach (LayerProperties property in _properties)
                    {
                        _layerPropertiesLookup.Add(property.ItemName, property);
                    }
                }
                return _layerPropertiesLookup[itemName];
            }
        }


        [System.Serializable]
        public class LayerProperties
        {
            public ItemName ItemName;
            [SortingLayer]
            public int SortingLayer;
            public int OrderInLayer;
        }
        public enum ItemName
        {
            Bead,
            BeadBurstEffect,
            BeadBurstParticle,
            CombineBeads,
            RectangleBeadCombinationEffect
        }
    }
}

