using SC.Core.UI;
using UnityEngine;

namespace MenuScene.MenuCanvas.View
{
    public interface INavigationCanvasView
    {
        public SpriteCanvas SpriteCanvas { get; }
        public void Show();
        public void Hide();
    }

    public class NavigationCanvasView : MonoBehaviour, INavigationCanvasView
    {
        [SerializeField] private SpriteCanvas _spriteCanvas;

        public SpriteCanvas SpriteCanvas => _spriteCanvas;

        public void Hide()
        {
            _spriteCanvas.HideAllUIs();
        }

        public void Show()
        {
            _spriteCanvas.ShowAllUIs();
        }
    }
}