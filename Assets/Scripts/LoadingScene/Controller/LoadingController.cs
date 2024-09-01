using Global.Controller;
using LoadingScene.View;
using Signals;
using Zenject;

namespace LoadingScene.Controller
{
    public class LoadingController
    {
        private readonly LoadingView _loadingView;

        public LoadingController(LoadingView loadingView, SignalBus signalBus)
        {
            _loadingView = loadingView;
            signalBus.Subscribe<GameStateReaction>(OnReaction);
        }

        private void OnReaction(GameStateReaction reaction)
        {
            switch (reaction.GameStatus)
            {
                case GameStatus.Menu:
                case GameStatus.Game:
                case GameStatus.LevelGenerator:
                    Hide();
                    break;
                default:
                    Show();
                    break;
            }
        }

        public void Hide() => _loadingView.Hide();
        public void Show() => _loadingView.Show();
    }
}