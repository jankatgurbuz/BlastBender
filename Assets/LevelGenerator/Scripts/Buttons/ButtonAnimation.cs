using Cysharp.Threading.Tasks;
using SC.Core.UI;
using UnityEngine;

namespace LevelGenerator.Scripts.Buttons
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

        private async void OnDown()
        {
            await ChangeColorOverTime(_spriteRenderer, _tempColor, 0.1f);
        }

        private async void OnClick()
        {
            await ChangeColorOverTime(_spriteRenderer, _currentColor, 0.1f);
        }

        private async UniTask ChangeColorOverTime(SpriteRenderer target, Color targetColor, float duration)
        {
            var startColor = target.color;
            float elapsedTime = 0;

            while (elapsedTime < duration)
            {
                target.color = Color.Lerp(startColor, targetColor, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                await UniTask.Yield(PlayerLoopTiming.Update);
            }

            target.color = targetColor;
        }
    }
}