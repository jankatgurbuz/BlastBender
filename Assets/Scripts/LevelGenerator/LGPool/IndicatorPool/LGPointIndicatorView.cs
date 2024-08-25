using UnityEngine;
using Util.Pool;

namespace LevelGenerator.LGPool.IndicatorPool
{
    public class LGPointIndicatorView : MonoBehaviour, IPoolable
    {
        private SpriteRenderer _spriteRenderer;
        public GameObject GameObject { get; private set; }
        public Transform Transform { get; private set; }

        public void Awake()
        {
            Transform = transform;
            GameObject = gameObject;
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void SetPosition(Vector3 vec)
        {
            Transform.position = vec;
        }

        public void SetColor(Color color)
        {
            _spriteRenderer.color = color;
        }
    }
}