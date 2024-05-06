using UnityEngine;

namespace SpriteCanvasSystem
{
    public abstract class UIElement : MonoBehaviour
    {
        [SerializeField] protected Transform _itemPosition;
        [SerializeReference] protected IResponsiveOperation _responsiveOperation;
        [SerializeField] protected SpriteRenderer _referenceSprite;
        [SerializeField] protected int _orderOffset;

        private Camera _camera;
        protected Camera Camera => _camera;

        public bool Interactable = true;

        public virtual void Initialize(Camera camera)
        {
            _camera = camera;
        }

        public string SpriteOperationName { get; } = nameof(_responsiveOperation);
        public IResponsiveOperation ResponsiveOperation => _responsiveOperation;

        public abstract void Handle(float screenHeight, float screenWidth, Camera camera, float _referenceOrthographicSize);
        public abstract void ArrangeLayers(string sortingLayer,int sortingOrder);
    }
}