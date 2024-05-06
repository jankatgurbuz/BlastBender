using BoardItems.LevelData;
using Cysharp.Threading.Tasks;
using Global.Controller;
using Signals;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

namespace Blast.Controller
{
    public interface IInGameController
    {
        LevelData LevelData { get; }
    }

    public class InGameController : IInGameController, IStartable
    {
        private readonly ISpriteCanvasController _spriteCanvasController;

        public LevelData LevelData { get; private set; }

        public InGameController(SignalBus signalBus, ISpriteCanvasController spriteCanvasController)
        {
            signalBus.Subscribe<GameStateReaction>(OnReaction);
            _spriteCanvasController = spriteCanvasController;
        }

        private void OnReaction(GameStateReaction reaction)
        {
            switch (reaction.GameStatus)
            {
                case GameStatus.Menu:
                    _spriteCanvasController.Disable();
                    break;
                case GameStatus.Game:
                    _spriteCanvasController.Enable();
                    break;
                case GameStatus.LevelGenerator:
                    _spriteCanvasController.Disable();
                    break;
            }
        }

        public async UniTask Start()
        {
            // todo: this will be updated with save system !
            
            const string assetPath = "Assets/Levels/Level1.asset";
            var locationHandle = Addressables.LoadResourceLocationsAsync(assetPath);
            await locationHandle.ToUniTask();
            if (locationHandle.Result.Count > 0)
            {
                LevelData = await Addressables.LoadAssetAsync<LevelData>(assetPath);
            }
            else
            {
                Debug.LogWarning("Scene not found in build settings: " + assetPath);
            }
        }
    }
}