using System.Collections;
using System.Collections.Generic;
using Global.View;
using Global.Controller;
using UnityEngine;
using Zenject;
using DG.Tweening;
using Cysharp.Threading.Tasks;

namespace Util.Pool.BeadEffect
{
    public class BeadBurstEffectView : MonoBehaviour, IPoolable
    {
        [SerializeField]
        private SpriteRenderer _spriteRenderer;

        private Transform _transform;
        private GameObject _gameObject;
        private LayersController _layersController;

        private Color _currentColor;
        private Color _customAlphaColor;
        private Color _zeroAlphaColor;

        private Vector3 _currentScale;
        private Vector3 _customScale;
        private Vector3 _zeroScale = Vector3.zero;

        public void Awake()
        {
            _transform = transform;
            _gameObject = gameObject;
        }

        public void Create()
        {
            _layersController ??= ProjectContext.Instance.Container.Resolve<LayersController>();
            var info = _layersController.GetLayerInfo(LayersProperties.ItemName.BeadBurstEffect);
            _spriteRenderer.sortingLayerID = info.SortingLayer;
            _spriteRenderer.sortingOrder = info.OrderInLayer;

            _currentColor = _spriteRenderer.color;
            _zeroAlphaColor = _currentColor;
            _customAlphaColor = _currentColor;
            _zeroAlphaColor.a = 0;
            _customAlphaColor.a = 0.2f;

            _currentScale = _transform.localScale;
            _customScale = _currentScale * 0.6f;
        }

        public void Active()
        {
            Handle();
        }

        public void Inactive()
        {
            _spriteRenderer.color = _currentColor;
            _transform.localScale = _currentScale;
        }

        public GameObject GetGameObject()
        {
            return _gameObject;
        }

        public Transform GetTransform()
        {
            return _transform;
        }

        public void SetPosition(Vector3 pos)
        {
            _transform.position = pos;
        }
        private async void Handle()
        {
            var firstScaleTime = 0.1f;
            var firstColorTime = 0.1f;
            var firstMovementTime = 0.5f;

            var scaleTweener = _transform.DOScale(_customScale, firstScaleTime);
            var colorTweener = _spriteRenderer.DOColor(_customAlphaColor, firstColorTime);
            var positionTweener = _transform.DOMoveY(0.2f, firstMovementTime).SetRelative();

            await UniTask.WhenAll(
                scaleTweener.AsyncWaitForCompletion().AsUniTask(),
                colorTweener.AsyncWaitForCompletion().AsUniTask()
                );

            //second 
            var secondScaleTime = 0.1f;
            var secondColorTime = 0.1f;

            scaleTweener = _transform.DOScale(_zeroScale, secondScaleTime);
             colorTweener = _spriteRenderer.DOColor(_zeroAlphaColor, secondColorTime);

            await UniTask.WhenAll(
               scaleTweener.AsyncWaitForCompletion().AsUniTask(),
               colorTweener.AsyncWaitForCompletion().AsUniTask(),
               positionTweener.AsyncWaitForCompletion().AsUniTask());

            BeadBurstEffectPool.Instance.Return(this);
        }
    }
}

