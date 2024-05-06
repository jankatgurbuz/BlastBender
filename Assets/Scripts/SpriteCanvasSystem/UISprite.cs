using UnityEngine;
namespace SpriteCanvasSystem
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class UISprite : UIElement
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;

        public override void ArrangeLayers(string sortingLayer, int sortingOrder)
        {
            _spriteRenderer.sortingLayerName = sortingLayer;
            _spriteRenderer.sortingOrder = sortingOrder + _orderOffset;
        }

        public override void Handle(float screenHeight, float screenWidth, Camera camera, float referenceOrthographicSize)
        {
            if (_referenceSprite == null)
            {
                var cp = camera.transform.position;
                var cameraPos = new Vector3(cp.x, cp.y, 0);

                _responsiveOperation.Handle(screenHeight, screenWidth,
                    _spriteRenderer.sprite.bounds.size, _itemPosition,
                    cameraPos, camera.orthographicSize, referenceOrthographicSize);
            }
            else
            {
                var size = _referenceSprite.sprite.bounds.size;

                _responsiveOperation.Handle(
                    size.y * _referenceSprite.transform.localScale.y,
                    size.x * _referenceSprite.transform.localScale.x,
                    _spriteRenderer.sprite.bounds.size, _itemPosition,
                    _referenceSprite.transform.position,
                    camera.orthographicSize,
                    referenceOrthographicSize);
            }
        }
    }
}
