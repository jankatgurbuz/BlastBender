using Global.Controller;
using Signals;
using UnityEngine;
using Zenject;

namespace Blast.Controller
{
    public class CameraController : BaseCameraController
    {
        private readonly IInGameController _inGameController;

        public CameraController(SignalBus signalBus, Camera camera, IGridController gridController,
            IInGameController inGameController) : base(camera, gridController)
        {
            _inGameController = inGameController;
            signalBus.Subscribe<GameStateReaction>(OnReaction);
        }

        private void OnReaction(GameStateReaction reaction)
        {
            if (reaction.GameStatus == GameStatus.GameInitialize)
            {
                var levelData = _inGameController.LevelData;
                var rowLength = levelData.RowLength;
                var columnLength = levelData.ColumnLength;
                FitCameraToGrid(rowLength, columnLength);
                SetOrto(columnLength);
            }
        }
    }
}