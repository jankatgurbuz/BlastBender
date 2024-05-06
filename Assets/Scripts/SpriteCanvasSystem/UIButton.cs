using System;

using UnityEngine;
using UnityEngine.Events;
using Util.Interaction;
using Util.Pool;

namespace SpriteCanvasSystem
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class UIButton : UIElement
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private UnityEvent<Vector3> _onClick;
        [SerializeField] private UnityEvent<Vector3> _onDown;

        public UnityEvent<Vector3> OnClick { get => _onClick; }
        public UnityEvent<Vector3> OnDown { get => _onDown; }

        private bool isReceiver = false;
        public override void Initialize(Camera camera)
        {
            base.Initialize(camera);

            if (Application.isPlaying && !isReceiver)
            {
                InteractionSystem.Instance.Receiver(Interaction);
                isReceiver = true;
            }
        }

        private void Interaction(InteractionPhase phase, Vector2 vector)
        {
            if (!Interactable)
                return;

            if (phase == InteractionPhase.None)
                return;

            var cameraPosition = Camera.ScreenToWorldPoint(vector);
            cameraPosition = new Vector3(cameraPosition.x, cameraPosition.y, _spriteRenderer.bounds.center.z);

            if (!_spriteRenderer.bounds.Contains(cameraPosition))
                return;

            switch (phase)
            {
                case InteractionPhase.Down:
                    _onDown?.Invoke(vector);
                    break;
                case InteractionPhase.Up:
                    _onClick?.Invoke(vector);
                    break;
            }
        }

        public override void ArrangeLayers(string sortingLayer, int sortingOrder)
        {
            _spriteRenderer.sortingLayerName = sortingLayer;
            _spriteRenderer.sortingOrder = sortingOrder + _orderOffset;
        }

        public override void Handle(float screenHeight, float screenWidth, Camera camera, float referenceOrthographicSize)
        {
            if (_referenceSprite == null)
            {
                var cp = camera.transform.position;
                var cameraPos = new Vector3(cp.x, cp.y, 0);

                _responsiveOperation.Handle(screenHeight, screenWidth,
                    _spriteRenderer.sprite.bounds.size, _itemPosition,
                    cameraPos, camera.orthographicSize, referenceOrthographicSize);
            }
            else
            {
                var size = _referenceSprite.sprite.bounds.size;

                _responsiveOperation.Handle(
                    size.y * _referenceSprite.transform.localScale.y,
                    size.x * _referenceSprite.transform.localScale.x,
                    _spriteRenderer.sprite.bounds.size, _itemPosition,
                    _referenceSprite.transform.position,
                    camera.orthographicSize,
                    referenceOrthographicSize);
            }
        }
    }
}
