using DG.Tweening;
using SC.Core.UI;
using UnityEngine;

namespace LevelGenerator.Buttons
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
        
            _currentColor = _spriteRenderer.color;
            _tempColor = _currentColor;

            _tempColor.r *= 0.5f;
            _tempColor.g *= 0.5f;
            _tempColor.b *= 0.5f;

            _button.ClickEvent.AddListener(OnClick);
            _button.DownEvent.AddListener(OnDown);
        }

        private void OnDown()
        {
            _spriteRenderer.DOColor(_tempColor, 0.1f);
        }

        private void OnClick()
        {
            _spriteRenderer.DOKill();
            _spriteRenderer.DOColor(_currentColor, 0.1f);
        }
    }
}