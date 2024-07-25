using BoardItems;
using BoardItems.Util;
using Cysharp.Threading.Tasks;
using Global.Controller;
using Global.View;
using UnityEngine;
using Util.Pool.BeadEffect;
using Zenject;

namespace Util.Pool.Bead
{
    public class BeadView : MonoBehaviour, IPoolable, IItemBehavior, IMoveable
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private BeadSettings _beadSettings;

        private GameObject _gameObject;
        private ItemColors _itemColor;
        private LayersController _layersController;

        public TransformUtilities TransformUtilities { get; set; }

        public void Awake()
        {
            _gameObject = gameObject;
            TransformUtilities = new TransformUtilities(transform);
        }

        public void Create()
        {
            _layersController ??= ProjectContext.Instance.Container.Resolve<LayersController>();
        }

        public void Active()
        {
        }

        public void Inactive()
        {
            TransformUtilities.ResetItem();
        }

        public GameObject GetGameObject()
        {
            return _gameObject;
        }

        public Transform GetTransform()
        {
            return TransformUtilities;
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
        
        public void Blast()
        {
            var beadBurstEffectItem = BeadBurstEffectPool.Instance.Retrieve();
            beadBurstEffectItem.SetPosition(TransformUtilities.GetPosition());

            var beadBurstParticleItem = BeadBurstParticlePool.Instance.Retrieve();
            beadBurstParticleItem.Burst(_itemColor, TransformUtilities.GetPosition());
        }

        public async UniTask CombineBead(int row, int column, int rowOffset, int columnOffset)
        {
            SetLayer(LayersProperties.ItemName.CombineBeads, row, columnOffset);

            const float moveTime = 0.15f;
            const float offset = 0.3f;
            var tempPos = TransformUtilities.GetPosition();
            var x = tempPos.x - columnOffset * offset;
            var y = tempPos.y - rowOffset * offset;
            var movePosition = new Vector3(x, y, tempPos.z);

            var effect = RectangleBeadCombinationEffectPool.Instance.Retrieve();

            effect.Movement(movePosition, moveTime, TransformUtilities.GetPosition()).Forget();
            
            float elapsedTime = 0;
            var startPosition = transform.position;

            while (elapsedTime < moveTime)
            {
                transform.position = Vector3.Lerp(startPosition, movePosition, elapsedTime / moveTime);
                elapsedTime += Time.deltaTime;
                await UniTask.Yield(PlayerLoopTiming.Update);
            }
            transform.position = movePosition;

            x = tempPos.x + columnOffset;
            y = tempPos.y + rowOffset;
            movePosition = new Vector3(x, y, tempPos.z);

            effect.Movement(movePosition, moveTime, TransformUtilities.GetPosition()).Forget();
            
            elapsedTime = 0;
            startPosition = transform.position;

            while (elapsedTime < moveTime)
            {
                transform.position = Vector3.Lerp(startPosition, movePosition, elapsedTime / moveTime);
                elapsedTime += Time.deltaTime;
                await UniTask.Yield(PlayerLoopTiming.Update);
            }
            transform.position = movePosition;

            RectangleBeadCombinationEffectPool.Instance.Return(effect);
        }
    }
}