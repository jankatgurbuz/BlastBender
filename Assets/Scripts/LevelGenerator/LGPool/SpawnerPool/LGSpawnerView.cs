using UnityEngine;
using Util.Pool;

namespace LevelGenerator.View
{
    public class LGSpawnerView : MonoBehaviour, IPoolable
    {
        private GameObject _gameObject;
        private Transform _transform;

        public void Awake()
        {
            _gameObject = gameObject;
            _transform = transform;
        }

        public void Create()
        {
        }

        public void Active()
        {
        }

        public void Inactive()
        {
        }
        public Transform GetTransform()
        {
            return _transform;
        }

        public GameObject GetGameObject()
        {
            return _gameObject;
        }

        
    }
}