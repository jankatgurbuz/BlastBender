using System.Collections.Generic;
using Global.Controller;
using SC.Core.UI;
using Signals;
using UnityEngine;
using Zenject;

namespace Blast.Controller
{
    public class CameraController : BaseCameraController
    {
        private readonly IInGameController _inGameController;
        private List<SpriteCanvas> _canvases;

        public CameraController(SignalBus signalBus, Camera camera, IGridController gridController,
            IInGameController inGameController, List<SpriteCanvas> canvases) : base(camera, gridController)
        {
            _inGameController = inGameController;
            _canvases = canvases;
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
                Camera.gameObject.SetActive(true);
                _canvases.ForEach(x => { x.AdjustDependentUIElements(); });
            }
            else if (reaction.GameStatus == GameStatus.MenuInitialize)
            {
                Camera.gameObject.SetActive(false);
            }
        }
    }
}