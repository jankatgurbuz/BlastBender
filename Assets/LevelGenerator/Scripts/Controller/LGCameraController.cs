using Blast.Controller;
using UnityEngine;

namespace LevelGenerator.Scripts.Controller
{
    public class LGCameraController : BaseCameraController
    {
        private readonly ILevelGeneratorController _levelGeneratorController;

        public LGCameraController(Camera camera, LGGridController gridController,
            ILevelGeneratorController levelGeneratorController) : base(camera, gridController)
        {
            _levelGeneratorController = levelGeneratorController;
            _levelGeneratorController.OnChangeState += FitCameraToGrid;
        }

        public void FitCameraToGrid()
        {
            FitCameraToGrid(_levelGeneratorController.RowLength, _levelGeneratorController.ColumnLength, true);
        }

        public void CreateArea()
        {
            Camera.orthographicSize = 20;
            FitCameraToGrid(_levelGeneratorController.RowLength, _levelGeneratorController.ColumnLength + 20,
                true);
        }
    }
}