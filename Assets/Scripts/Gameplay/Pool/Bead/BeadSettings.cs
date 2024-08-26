using System.Collections.Generic;
using System.Linq;
using BoardItems;
using UnityEngine;

namespace Gameplay.Pool.Bead
{
    [CreateAssetMenu(fileName = "BeadSettings", menuName = "BlastBender/Game/BeadSettings")]
    public class BeadSettings : ScriptableObject
    {
        [SerializeField] private List<SpriteTable> _spriteTableList;
        private Dictionary<ItemColors, Sprite> _dictionary;

        public Sprite this[ItemColors key]
        {
            get
            {
                if (_dictionary == null)
                {
                    CreateDictionary();
                }
                if (_dictionary.TryGetValue(key, out var sprite))
                {
                    return sprite;
                }
                throw new KeyNotFoundException($"The key {key} was not found in the sprite dictionary.");
            }
        }

        private void CreateDictionary()
        {
            _dictionary = _spriteTableList.ToDictionary(item => item.Key, item => item.Sprite);
        }

        [System.Serializable]
        public class SpriteTable
        {
            public ItemColors Key;
            public Sprite Sprite;
        }
    }
}
