using TMPro;
using UnityEngine;


namespace SpriteCanvasSystem
{
    [RequireComponent(typeof(TextMeshPro))]
    public class UITextMeshPro :  UIElement
    {
        [SerializeField] private TextMeshPro _textMeshPro;

        public override void ArrangeLayers(string sortingLayer, int sortingOrder)
        {
            _textMeshPro.sortingLayerID = SortingLayer.NameToID(sortingLayer);
            _textMeshPro.sortingOrder = sortingOrder + _orderOffset;
        }

        public override void Handle(float screenHeight, float screenWidth, Camera camera, float referenceOrthographicSize)
        {
            if (_referenceSprite == null)
            {
                var cp = camera.transform.position;
                var cameraPos = new Vector3(cp.x, cp.y, 0);

                _responsiveOperation.Handle(screenHeight, screenWidth, _textMeshPro.bounds.size,
                    _itemPosition, cameraPos, camera.orthographicSize, referenceOrthographicSize);
            }
            else
            {
                var size = _referenceSprite.sprite.bounds.size;

                _responsiveOperation.Handle(
                    size.y * _referenceSprite.transform.localScale.y,
                    size.x * _referenceSprite.transform.localScale.x,
                    _textMeshPro.bounds.size, _itemPosition,
                    _referenceSprite.transform.position,
                    camera.orthographicSize, referenceOrthographicSize);
            }
        }
    }
}
