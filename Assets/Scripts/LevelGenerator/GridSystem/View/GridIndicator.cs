using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelGenerator.GridSystem.View
{
    public class GridIndicator : MonoBehaviour
    {
        private SpriteRenderer _spriteRenderer;
        private Transform _transform;

       

        public void SetPosition(Vector3 vec)
        {
            _transform.position = vec;
        }

        private void Awake()
        {
            _transform = transform;
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }
        public void SetColor(Color color)
        {
            _spriteRenderer.color = color;
        }
    }
}
