using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace LoadingScene.View
{
    public class LoadingView : MonoBehaviour
    {
        [SerializeField] private LoadingBackgroundReferance _backgroundReferance;
        [SerializeField] private Transform _backgroundParent;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private GameObject _tempBackground;
        [SerializeField] private TextMeshProUGUI _tmpLoadingText;

        private readonly string[] _loadingTexts = {
            "Loading",
            "Loading.",
            "Loading..",
            "Loading..."
        };

        private CancellationTokenSource _loadingTextCancellation;
        private int _textIndex;

        public void Awake()
        {
            LoadBackgroundImage();
            LoadingTextSpinner();
        }

        public void Show()
        {
            if (_canvasGroup.alpha == 1) return;

            TweenAlpha(1, BlocksRaycasts);
            LoadingTextSpinner();
        }

        public void Hide()
        {
            _loadingTextCancellation?.Cancel();
            TweenAlpha(0, BlocksRaycasts);
        }
        private void BlocksRaycasts(int check)
        {
            _canvasGroup.blocksRaycasts = check == 0;
        }
        private async void TweenAlpha(int alpha, Action<int> finishAction)
        {
            var elapsedTime = 0f;
            var fadeDuration = 0.1f;

            var xorAlpha = alpha ^ 1;
            _canvasGroup.alpha = xorAlpha;

            while (true)
            {
                _canvasGroup.alpha = Mathf.Lerp(xorAlpha, alpha, elapsedTime / fadeDuration);
                if (_canvasGroup.alpha == alpha) break;

                elapsedTime += Time.deltaTime;
                await UniTask.Yield(PlayerLoopTiming.LastUpdate);
            }
            finishAction.Invoke(xorAlpha);
        }
        private async void LoadingTextSpinner()
        {
            _loadingTextCancellation?.Cancel();
            _loadingTextCancellation = new CancellationTokenSource();
            while (!_loadingTextCancellation.Token.IsCancellationRequested)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
                _textIndex = (_textIndex + 1) % _loadingTexts.Length;
                _tmpLoadingText.text = _loadingTexts[_textIndex];
            }
        }

        private async void LoadBackgroundImage()
        {
            var gameObject = await _backgroundReferance.InstantiateAsync(_backgroundParent);
            gameObject.SetActive(false);
            await UniTask.WaitUntil(() => Mathf.Approximately(_canvasGroup.alpha, 0));
            gameObject.SetActive(true);
            Destroy(_tempBackground);
        }
    }
}
