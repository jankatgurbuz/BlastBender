using Cysharp.Threading.Tasks;
using Global.Controller;
using LoadingScene.View;
using Signals;
using Zenject;

namespace LoadingScene.Controller
{
    public interface ILoadingController
    {
        public void Show();
        public void Hide();
    }
    public class LoadingController : ILoadingController
    {
        private readonly LoadingView _loadingView;
        private readonly SignalBus _signalBus;

        public LoadingController(LoadingView loadingView, SignalBus signalBus,GameController gameController)
        {
            _loadingView = loadingView;
            _signalBus = signalBus;
            _signalBus.Subscribe<GameStateReaction>(OnReaction);
            gameController.LoadNextScene().Forget();
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
