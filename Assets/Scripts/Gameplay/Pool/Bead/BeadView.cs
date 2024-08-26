using BoardItems;
using BoardItems.Util;
using Gameplay.Pool.BeadEffect;
using Global.Controller;
using UnityEngine;
using Zenject;

namespace Gameplay.Pool.Bead
{
    public class BeadView : MonoBehaviour, IPoolable, IInitializable,IDeactivatable, IItemUtility
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private BeadSettings _beadSettings;
        
        private ItemColors _color;
        private LayersController _layersController;
        public TransformUtilities TransformUtilities { get; set; }
        public GameObject GameObject { get; private set; }
        public Transform Transform => TransformUtilities;

        public ItemColors Color
        {
            get => _color;
            set
            {
                _color = value;
                _spriteRenderer.sprite = _beadSettings[_color];
            }
        }

        public void Awake()
        {
            GameObject = gameObject;
            TransformUtilities = new TransformUtilities(transform);
        }

        public void Initialize()
        {
            _layersController ??= ProjectContext.Instance.Container.Resolve<LayersController>();
        }
        public void Deactivate()
        {
            TransformUtilities.ResetItem();
        }
        public void SetActive(bool active)
        {
            GameObject.SetActive(active);
        }

        public void SetSortingOrder(string layerKey, int row, int column)
        {
            var info = _layersController.GetLayerInfo(layerKey);
            _spriteRenderer.sortingLayerID = info.SortingLayer;
            _spriteRenderer.sortingOrder = info.OrderInLayer + (row + column);
        }

        public void Blast()
        {
            var beadBurstEffectItem = BeadBurstEffectPool.Instance.Retrieve();
            beadBurstEffectItem.SetPosition(TransformUtilities.GetPosition());

            var beadBurstParticleItem = BeadBurstParticlePool.Instance.Retrieve();
            beadBurstParticleItem.Burst(Color, TransformUtilities.GetPosition());
        }
    }
}