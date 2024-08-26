using Global.Controller;
using UnityEngine;
using Zenject;

namespace Gameplay.Pool.BeadEffect
{
    public class RectangleBeadCombinationEffectView : MonoBehaviour, IPoolable, IInitializable
    {
        private const string LayerKey = "RectangleBeadCombinationEffect";

        [SerializeField] private SpriteRenderer _spriteRenderer;
        public GameObject GameObject { get; private set; }
        public Transform Transform { get; private set; }

        private LayersController _layerController;

        public void Awake()
        {
            Transform = transform;
            GameObject = gameObject;
        }

        public void Initialize()
        {
            _layerController ??= ProjectContext.Instance.Container.Resolve<LayersController>();
            var info = _layerController.GetLayerInfo(LayerKey);

            _spriteRenderer.sortingLayerID = info.SortingLayer;
            _spriteRenderer.sortingOrder = info.OrderInLayer;
        }

        public void SetPosition(Vector3 movePosition)
        {
            Transform.position = movePosition;
        }
    }
}