using Cysharp.Threading.Tasks;
using Global.Controller;
using Zenject;

namespace LoadingScene.Controller
{
    public class LoadingSceneInitializer : IInitializable
    {
        private readonly GameController _gameController;

        public LoadingSceneInitializer(GameController gameController)
        {
            _gameController = gameController;
        }

        public void Initialize()
        {
            _gameController.LoadNextScene().Forget();
        }
    }
}