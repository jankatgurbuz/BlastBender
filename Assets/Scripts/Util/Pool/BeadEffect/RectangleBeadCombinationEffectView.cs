using Cysharp.Threading.Tasks;
using Global.Controller;
using UnityEngine;
using Zenject;

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
            
        }

        public async UniTask Movement(Vector3 movePosition, float moveTime, Vector3 startPosition)
        {
            _transform.position = startPosition;
            await MoveOverTime(_transform, movePosition, moveTime);
        }

        private async UniTask MoveOverTime(Transform target, Vector3 endPosition, float duration)
        {
            var startPosition = target.position;
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
