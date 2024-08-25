using BoardItems;
using BoardItems.Util;
using UnityEngine;

namespace Util.Pool.VoidItem
{
    public class VoidPoolView : MonoBehaviour, IPoolable, IItemBehavior
    {
        public TransformUtilities TransformUtilities { get; set; }
        public GameObject GameObject { get; private set; }
        public Transform Transform { get; private set; }

        public void Awake()
        {
            Transform = transform;
            GameObject = gameObject;
        }

        public void SetActive(bool active)
        {
            GameObject.SetActive(active);
        }

        public void Blast()
        {
        }
    }
}