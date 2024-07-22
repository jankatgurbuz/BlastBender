using BoardItems;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Global.Controller;
using Global.View;
using UnityEngine;
using Util.Handlers;
using Util.Handlers.Strategies;
using Util.Pool.BeadEffect;
using Zenject;

namespace Util.Pool.Bead
{
    public class BeadView : MonoBehaviour, IPoolable, IItemBehavior, IMoveable
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private BeadSettings _beadSettings;

        private Transform _transform;
        private GameObject _gameObject;
        private ItemColors _itemColor;
        private LayersController _layersController;

        private Quaternion _currentRotation;
        private Vector3 _currentScale;

        public void Awake()
        {
            _transform = transform;
            _gameObject = gameObject;
        }

        public void Create()
        {
            _currentRotation = _transform.rotation;
            _currentScale = transform.localScale;
            _layersController ??= ProjectContext.Instance.Container.Resolve<LayersController>();
        }

        public void Active()
        {
        }

        public void Inactive()
        {
            _transform.DOKill();
            ResetItem();
        }

        public GameObject GetGameObject()
        {
            return _gameObject;
        }

        public Transform GetTransform()
        {
            return _transform;
        }

        private void ResetItem()
        {
            _transform.localScale = _currentScale;
            _transform.rotation = _currentRotation;
        }

        public void SetPosition(Vector3 position)
        {
            _transform.position = position;
        }

        public void SetColorAndAddSprite(ItemColors color)
        {
            _itemColor = color;
            _spriteRenderer.sprite = _beadSettings[color];
        }

        public void SetActive(bool active)
        {
            _gameObject.SetActive(active);
        }

        public void SetSortingOrder(int row, int column)
        {
            SetLayer(LayersProperties.ItemName.Bead, row, column);
        }

        private void SetLayer(LayersProperties.ItemName item, int row, int column)
        {
            var info = _layersController.GetLayerInfo(item);
            _spriteRenderer.sortingLayerID = info.SortingLayer;
            _spriteRenderer.sortingOrder = info.OrderInLayer + (row + column);
        }

        public Vector3 GetPosition()
        {
            return _transform.position;
        }

        public void Blast()
        {
            var beadBurstEffectItem = BeadBurstEffectPool.Instance.Retrieve();
            beadBurstEffectItem.SetPosition(_transform.position);

            var beadBurstParticleItem = BeadBurstParticlePool.Instance.Retrieve();
            beadBurstParticleItem.Burst(_itemColor, _transform.position);
        }

        public void Shake(IMovementStrategy strategy)
        {
            if (strategy.IsPlayShake) return;

            strategy.Shake2(_transform);
        }

        public void StartMovement(IMovementStrategy strategy)
        {
            if (strategy.IsPlayShake || strategy.IsPlayFinalMovement)
            {
                strategy.Kill = true;
            }

            strategy.StartMovement2(_transform);
        }

        public void FinalizeMovementWithBounce(IMovementStrategy strategy)
        {
            if (strategy.IsPlayFinalMovement) return;

            strategy.FinalMovement2(_transform, _currentScale);
        }

        public async UniTask CombineBead(int row, int column, int rowOffset, int columnOffset)
        {
            SetLayer(LayersProperties.ItemName.CombineBeads, row, columnOffset);

            const float moveTime = 0.15f;
            const float offset = 0.3f;
            var tempPos = _transform.position;
            var x = tempPos.x - columnOffset * offset;
            var y = tempPos.y - rowOffset * offset;
            var movePosition = new Vector3(x, y, tempPos.z);

            var effect = RectangleBeadCombinationEffectPool.Instance.Retrieve();

            effect.Movement(movePosition, moveTime, _transform.position);
            await _transform.DOMove(movePosition, moveTime).AsyncWaitForCompletion().AsUniTask();

            x = tempPos.x + columnOffset;
            y = tempPos.y + rowOffset;
            movePosition = new Vector3(x, y, tempPos.z);

            effect.Movement(movePosition, moveTime, _transform.position);
            await _transform.DOMove(movePosition, moveTime).AsyncWaitForCompletion().AsUniTask();

            RectangleBeadCombinationEffectPool.Instance.Return(effect);
        }
    }
}