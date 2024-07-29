using Cysharp.Threading.Tasks;
using Global.Controller;
using Zenject;

namespace MenuScene.Controller
{
    public class MainMenuInitializer : IInitializable
    {
        private GameController _gameController;

        public MainMenuInitializer(GameController gameController)
        {
            _gameController = gameController;
        }

        public void Initialize()
        {
            _gameController.LoadNextScene().Forget();
        }
    }
}