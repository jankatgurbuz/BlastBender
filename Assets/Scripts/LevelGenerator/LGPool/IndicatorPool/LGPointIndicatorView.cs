using UnityEngine;
using Util.Pool;

namespace LevelGenerator.LGPool.IndicatorPool
{
    public class LGPointIndicatorView : MonoBehaviour,IPoolable
    {
        private Transform _transform;
        private GameObject _gameObject;
        private SpriteRenderer _spriteRenderer;

        public void Awake()
        {
            _transform = transform;
            _gameObject = gameObject;
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public GameObject GetGameObject()
        {
            return _gameObject;
        }

        public Transform GetTransform()
        {
            return _transform;
        }

        public void Active()
        {
        }

        public void Create()
        {
        }

        public void Inactive()
        {
        }
        public void SetPosition(Vector3 vec)
        {
            _transform.position = vec;
        }
        public void SetColor(Color color)
        {
            _spriteRenderer.color = color;
        }
    }
}
