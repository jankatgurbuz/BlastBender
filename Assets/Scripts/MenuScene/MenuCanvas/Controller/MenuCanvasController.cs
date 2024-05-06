using System;
using Cysharp.Threading.Tasks;
using Global.Controller;
using MenuScene.NavigatorPanel.View;
using MenuScene.MenuCanvas.View;
using Zenject;
using Signals;

namespace MenuScene.MenuCanvas.Controller
{
    public interface IMenuCanvasController
    {
        public void Show();
        public void Hide();
    }
    public class MenuCanvasController : IMenuCanvasController
    {
        private SignalBus _signalBus;
        private readonly IMenuCanvas _menuPanelsHandler;
        private readonly INavigationPanelController _navigationPanelController;

        public MenuCanvasController(SignalBus signalBus, IMenuCanvas menuPanelsHandler, INavigationPanelController navigationPanelController)
        {
            _menuPanelsHandler = menuPanelsHandler;
            _navigationPanelController = navigationPanelController;
            _signalBus = signalBus;
            _signalBus.Subscribe<GameStateReaction>(OnReaction);
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
            _menuPanelsHandler.Hide();
            
        }

        public void Show()
        {
            _navigationPanelController.DirectChange(2);
            _menuPanelsHandler.Show();
        }
    }
}
