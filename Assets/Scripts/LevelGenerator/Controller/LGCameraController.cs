using System.Collections;
using System.Collections.Generic;
using Blast.Controller;
using Blast.View;
using UnityEngine;

namespace LevelGenerator.Controller
{
    public class LGCameraController : BaseCameraController
    {
        private readonly ILevelGeneratorController _levelGeneratorController;

        public LGCameraController(Camera camera, IGridController gridController,
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