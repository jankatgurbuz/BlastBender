using System;
using Cysharp.Threading.Tasks;
using Global.Controller;

using MenuScene.MenuCanvas.View;
using Zenject;
using Signals;

namespace MenuScene.MenuCanvas.Controller
{
    public interface INavigationCanvasController
    {
        public void Show();
        public void Hide();
    }
    public class NavigationCanvasController : INavigationCanvasController
    {
         private readonly INavigationCanvasView _navigationCanvasView;

        public NavigationCanvasController(SignalBus signalBus,INavigationCanvasView navigationCanvasView,IMainMenuCameraController mainMenuCameraHandler)
        {
            _navigationCanvasView = navigationCanvasView;
            signalBus.Subscribe<GameStateReaction>(OnReaction);
            mainMenuCameraHandler.Register(_navigationCanvasView.SpriteCanvas);
        }
        private void OnReaction(GameStateReaction reaction)
        {
            switch (reaction.GameStatus)
            {
                case GameStatus.Menu:
                    Show();
                    break;
                case GameStatus.Game:
                    Hide();
                    break;
                case GameStatus.LevelGenerator:
                    Hide();
                    break;
            }
        }

        public void Hide()
        {
            _navigationCanvasView.Hide();
        }

        public void Show()
        {
            _navigationCanvasView.Show();
            // _navigationPanelController.DirectChange(2);
        }
    }
}
