using System.Collections;
using System.Collections.Generic;
using BoardItems;
using UnityEngine;

namespace Util.Pool.Border
{
    public class BorderView : MonoBehaviour, IPoolable
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;
        private Transform _transform;
        private GameObject _gameobject;

        public void Awake()
        {
            _transform = transform;
            _gameobject = gameObject;
        }

        public GameObject GetGameObject()
        {
            return _gameobject;
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

        public void SetSprite(Sprite sprite)
        {
            _spriteRenderer.sprite = sprite;
        }

        public void SetPosition(Vector3 vec)
        {
            _transform.position = vec;
        }
    }
}