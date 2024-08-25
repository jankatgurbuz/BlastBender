using BoardItems;
using BoardItems.Util;
using Global.Controller;
using UnityEngine;
using Zenject;

namespace Util.Pool.Duck
{
    public class DuckView : MonoBehaviour, IPoolable, IItemUtility, IInitializable
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;
        
        private LayersController _layersController;
        public TransformUtilities TransformUtilities { get; set; }
        public GameObject GameObject { get; private set; }
        public Transform Transform => TransformUtilities;

        public void Awake()
        {
            GameObject = gameObject;
            TransformUtilities = new TransformUtilities(transform);
        }

        public void Initialize()
        {
            _layersController ??= ProjectContext.Instance.Container.Resolve<LayersController>();
        }

        public void SetActive(bool active)
        {
            GameObject.SetActive(active);
        }

        public void Blast()
        {
        }

        public void SetSortingOrder(string layerKey, int row, int column)
        {
            var info = _layersController.GetLayerInfo(layerKey);
            _spriteRenderer.sortingLayerID = info.SortingLayer;
            _spriteRenderer.sortingOrder = info.OrderInLayer + (row + column);
        }
    }
}