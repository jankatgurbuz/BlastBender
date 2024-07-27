using BoardItems;
using BoardItems.Util;
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

        public void SetLayer(int row, int columnOffset)
        {
            SetLayer(LayersProperties.ItemName.CombineBeads, row, columnOffset);
        }
    }
}