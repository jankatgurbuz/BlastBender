using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Global.Controller;
using MenuScene.Controller;
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
        private readonly List<NavigationCanvasView> _navigationCanvasViews;

        public NavigationCanvasController(SignalBus signalBus, List<NavigationCanvasView> navigationCanvasViews)
        {
            _navigationCanvasViews = navigationCanvasViews;
            signalBus.Subscribe<GameStateReaction>(OnReaction);
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
            foreach (var canvasItem in _navigationCanvasViews)
            {
                canvasItem.Hide();
            }
        }

        public void Show()
        {
            foreach (var canvasItem in _navigationCanvasViews)
            {
                canvasItem.Show();
            }
        }
    }
}