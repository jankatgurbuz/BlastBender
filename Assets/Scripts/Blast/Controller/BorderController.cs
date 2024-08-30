using System;
using BoardItems.Border;
using Cysharp.Threading.Tasks;
using Global.Controller;
using Signals;
using Zenject;

namespace Blast.Controller
{
    public class BorderController : BaseBorderController, IStartable
    {
        private readonly IInGameController _inGameController;

        public BorderController(SignalBus signalBus, BorderProperties borderProperties,
            IGridController gridController, IInGameController inGameController)
            : base(borderProperties, gridController)
        {
            _inGameController = inGameController;
            signalBus.Subscribe<GameStateReaction>(PlayGame);
        }

        public async UniTask Start()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(0.01f));
        }

        private void PlayGame(GameStateReaction reaction)
        {
            if (reaction.GameStatus != GameStatus.GameInitialize) return;

            var levelData = _inGameController.LevelData;
            CreateBorderMatrix(levelData.RowLength, levelData.ColumnLength, levelData);
        }
    }
}