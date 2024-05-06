using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Global.Controller;
using UnityEngine;

namespace MenuScene.MenuCanvas.View
{
    public interface IMenuCanvas
    {
        public void Show();
        public void Hide();
    }
    public class MenuCanvas : MonoBehaviour, IMenuCanvas
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        public void Hide()
        {
            _canvasGroup.alpha = 0;
            _canvasGroup.blocksRaycasts = false;
        }

        public void Show()
        {
            _canvasGroup.alpha = 1;
            _canvasGroup.blocksRaycasts = true;
        }
    }
}
