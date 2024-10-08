using Cysharp.Threading.Tasks;
using Global.Controller;
using UnityEngine;
using Zenject;

namespace Gameplay.Pool.BeadEffect
{
    // Todo Change:The BeadBurstEffectView system must be changed
    public class BeadBurstEffectView : MonoBehaviour, IPoolable, IInitializable, IDeactivatable, IActivatable
    {
        private const string LayerKey = "BeadBurstEffect";

        [SerializeField] private SpriteRenderer _spriteRenderer;

        private LayersController _layersController;

        private Color _currentColor;
        private Color _customAlphaColor;
        private Color _zeroAlphaColor;

        private Vector3 _currentScale;
        private Vector3 _customScale;
        private Vector3 _zeroScale = Vector3.zero;

        public GameObject GameObject { get; private set; }
        public Transform Transform { get; private set; }

        public void Awake()
        {
            Transform = transform;
            GameObject = gameObject;
        }

        public void Initialize()
        {
            _layersController ??= ProjectContext.Instance.Container.Resolve<LayersController>();
            var info = _layersController.GetLayerInfo(LayerKey);
            _spriteRenderer.sortingLayerID = info.SortingLayer;
            _spriteRenderer.sortingOrder = info.OrderInLayer;

            _currentColor = _spriteRenderer.color;
            _zeroAlphaColor = _currentColor;
            _customAlphaColor = _currentColor;
            _zeroAlphaColor.a = 0;
            _customAlphaColor.a = 0.2f;

            _currentScale = Transform.localScale;
            _customScale = _currentScale * 0.6f;
        }

        public void Active()
        {
            Handle();
        }

        public void Deactivate()
        {
            _spriteRenderer.color = _currentColor;
            Transform.localScale = _currentScale;
        }

        public void SetPosition(Vector3 pos)
        {
            Transform.position = pos;
        }

        private async void Handle()
        {
            var firstScaleTime = 0.1f;
            var firstColorTime = 0.1f;
            var firstMovementTime = 0.1f;

            await ScaleOverTime(Transform, _customScale, firstScaleTime);
            await ChangeColorOverTime(_spriteRenderer, _customAlphaColor, firstColorTime);
            await MoveYOverTime(Transform, 0.2f, firstMovementTime);

            var secondScaleTime = 0.1f;
            var secondColorTime = 0.1f;

            await UniTask.WhenAll(
                ScaleOverTime(Transform, _zeroScale, secondScaleTime),
                ChangeColorOverTime(_spriteRenderer, _zeroAlphaColor, secondColorTime)
            );

            BeadBurstEffectPool.Instance.Return(this);
        }

        private async UniTask ScaleOverTime(Transform target, Vector3 targetScale, float duration)
        {
            var startScale = target.localScale;
            float elapsedTime = 0;

            while (elapsedTime < duration)
            {
                target.localScale = Vector3.Lerp(startScale, targetScale, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                await UniTask.Yield(PlayerLoopTiming.Update);
            }

            target.localScale = targetScale;
        }

        private async UniTask ChangeColorOverTime(SpriteRenderer target, Color targetColor, float duration)
        {
            var startColor = target.color;
            float elapsedTime = 0;

            while (elapsedTime < duration)
            {
                target.color = Color.Lerp(startColor, targetColor, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                await UniTask.Yield(PlayerLoopTiming.Update);
            }

            target.color = targetColor;
        }

        private async UniTask MoveYOverTime(Transform target, float relativeY, float duration)
        {
            var startPosition = target.position;
            var endPosition = new Vector3(startPosition.x, startPosition.y + relativeY, startPosition.z);
            float elapsedTime = 0;

            while (elapsedTime < duration)
            {
                target.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                await UniTask.Yield(PlayerLoopTiming.Update);
            }

            target.position = endPosition;
        }
    }
}