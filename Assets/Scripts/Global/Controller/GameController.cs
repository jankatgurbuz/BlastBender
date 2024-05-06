using System;
using Cysharp.Threading.Tasks;
using Zenject;
using UnityEngine.SceneManagement;
using Signals;

namespace Global.Controller
{
    public interface IGameController
    {
        bool InitializationComplete { get; set; }
        UniTask LoadNextScene(int? overrideIndex = null);
        UniTask MenuInitialize();
        UniTask GameInitialize();
        UniTask LevelGeneratorInitialize();
    }

    public enum GameStatus
    {
        Empty,
        Menu,
        Game,
        LevelGenerator,
        MenuInitialize,
        GameInitialize,
        LevelGeneratorInitialize
    }

    public class GameController : IStartable, IGameController
    {
        private readonly SignalBus _signalBus;
        private int? _firstBuildIndex;

        public bool InitializationComplete { get; set; } = false;

        public GameController(SignalBus signalBus)
        {
            _signalBus = signalBus;
        }

        public async UniTask Start()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(0.1f));
        }
        public async UniTask MenuInitialize()
        {
            _signalBus.Fire(new GameStateReaction(GameStatus.MenuInitialize));
            await UniTask.Delay(TimeSpan.FromSeconds(1));
            _signalBus.Fire(new GameStateReaction(GameStatus.Menu));
        }

        public async UniTask GameInitialize()
        {
            _signalBus.Fire(new GameStateReaction(GameStatus.GameInitialize));
            await UniTask.Delay(TimeSpan.FromSeconds(1));
            _signalBus.Fire(new GameStateReaction(GameStatus.Game));
        }

        public async UniTask LevelGeneratorInitialize()
        {
            await LoadNextScene(GetSceneIndex("LevelGenerator"));
            _signalBus.Fire(new GameStateReaction(GameStatus.LevelGeneratorInitialize));
            await UniTask.Delay(TimeSpan.FromSeconds(1));
            _signalBus.Fire(new GameStateReaction(GameStatus.LevelGenerator));

            int GetSceneIndex(string sceneName)
            {
                for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
                {
                    var path = SceneUtility.GetScenePathByBuildIndex(i);
                    var name = System.IO.Path.GetFileNameWithoutExtension(path);
                    if (name == sceneName)
                        return i;
                   
                }
                throw new Exception("Scene not found in build settings: " + sceneName);
            }
        }

        public async UniTask LoadNextScene(int? overrideIndex = null)
        {
            int index;
            if (overrideIndex == null)
            {
                _firstBuildIndex ??= SceneManager.GetActiveScene().buildIndex + 1;
                index = (int)_firstBuildIndex++;
            }
            else
            {
                index = (int)overrideIndex;
            }

            await SceneManager.LoadSceneAsync(index, LoadSceneMode.Additive);
        }
    }
}