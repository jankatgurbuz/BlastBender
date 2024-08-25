using BoardItems;
using BoardItems.Util;
using UnityEngine;

namespace Util.Pool.VoidItem
{
    public class VoidPoolView : MonoBehaviour, IPoolable, IItemBehavior
    {
        private Transform _transform;
        private GameObject _gameObject;

        public TransformUtilities TransformUtilities { get; set; }

        public void Awake()
        {
            _transform = transform;
            _gameObject = gameObject;
        }
        public GameObject GetGameObject()
        {
            return _gameObject;
        }

        public Transform GetTransform()
        {
            return _transform;
        }
        public void SetActive(bool active)
        {
            _gameObject.SetActive(active);
        }

        public void Blast()
        {
        }
    }
}