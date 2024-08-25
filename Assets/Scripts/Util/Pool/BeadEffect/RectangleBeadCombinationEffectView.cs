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

        private const string LayerKey = "RectangleBeadCombinationEffect";

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
            var info = _layerController.GetLayerInfo(LayerKey);

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

        public void SetPosition(Vector3 movePosition)
        {
            _transform.position = movePosition;
        }
    }
}