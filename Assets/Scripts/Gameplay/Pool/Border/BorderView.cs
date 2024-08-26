using UnityEngine;

namespace Gameplay.Pool.Border
{
    public class BorderView : MonoBehaviour, IPoolable
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;

        public GameObject GameObject { get; private set; }
        public Transform Transform { get; private set; }

        public void Awake()
        {
            Transform = transform;
            GameObject = gameObject;
        }

        public void SetSprite(Sprite sprite)
        {
            _spriteRenderer.sprite = sprite;
        }

        public void SetPosition(Vector3 vec)
        {
            Transform.position = vec;
        }
    }
}