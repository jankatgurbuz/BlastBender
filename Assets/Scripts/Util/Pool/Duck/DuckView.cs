using BoardItems;
using BoardItems.Util;
using Global.Controller;
using UnityEngine;
using Zenject;

namespace Util.Pool.Duck
{
    public class DuckView : MonoBehaviour, IPoolable, IItemBehavior,IInitializable
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;
        private GameObject _gameObject;
        private LayersController _layersController;
        public TransformUtilities TransformUtilities { get; set; }

        public void Awake()
        {
            _gameObject = gameObject;
            TransformUtilities = new TransformUtilities(transform);
        }

        public void Initialize()
        {
            _layersController ??= ProjectContext.Instance.Container.Resolve<LayersController>();
        }
        
        public GameObject GetGameObject()
        {
            return _gameObject;
        }

        public Transform GetTransform()
        {
            return TransformUtilities;
        }


        public void SetActive(bool active)
        {
            _gameObject.SetActive(active);
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