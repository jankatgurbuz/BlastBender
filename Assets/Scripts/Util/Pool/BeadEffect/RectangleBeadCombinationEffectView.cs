using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Global.Controller;
using UnityEngine;
using Zenject;
using static UnityEngine.Rendering.DebugUI.Table;

namespace Util.Pool.BeadEffect
{
    public class RectangleBeadCombinationEffectView : MonoBehaviour, IPoolable
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;
        private Transform _transform;
        private GameObject _gameObject;
        private LayersController _layerController;
        public void Awake()
        {
            _transform = transform;
            _gameObject = gameObject;
        }

        public void Active()
        {
            
        }

        public void Create()
        {
            _layerController ??= ProjectContext.Instance.Container.Resolve<LayersController>();
            var info = _layerController.GetLayerInfo(Global.View.LayersProperties.ItemName.RectangleBeadCombinationEffect);

            _spriteRenderer.sortingLayerID = info.SortingLayer;
            _spriteRenderer.sortingOrder = info.OrderInLayer;
        }

        public GameObject GetGameObject()
        {
            return _gameObject;
        }

        public Transform GetTransform()
        {
            return _transform;
        }

        public void Inactive()
        {
            _transform.DOKill();
        }

        public void Movement(Vector3 movePosition, float moveTime, Vector3 position)
        {
            _transform.position = position;
            _transform.DOMove(movePosition, moveTime);
        }
    }
}
