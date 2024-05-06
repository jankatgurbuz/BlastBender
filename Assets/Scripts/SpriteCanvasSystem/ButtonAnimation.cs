using UnityEngine;
using DG.Tweening;

namespace SpriteCanvasSystem
{
    public class ButtonAnimation : MonoBehaviour
    {
        private SpriteRenderer _spriteRenderer;
        private UIButton _button;
        private Color _currentColor, _tempColor;

        private void Start()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _button = GetComponent<UIButton>();

            // color is a struct type.
            _currentColor = _spriteRenderer.color;
            _tempColor = _currentColor;

            _tempColor.r *= 0.5f;
            _tempColor.g *= 0.5f;
            _tempColor.b *= 0.5f;

            _button.OnClick.AddListener(OnClick);
            _button.OnDown.AddListener(OnDown);
        }

        private void OnDown(Vector3 vec)
        {
            _spriteRenderer.DOColor(_tempColor, 0.1f);
        }

        private void OnClick(Vector3 vec)
        {
            _spriteRenderer.DOKill();
            _spriteRenderer.DOColor(_currentColor, 0.1f);
        }
    }
}
