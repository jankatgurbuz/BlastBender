using Blast.View;
using UnityEngine;
using DG.Tweening;

namespace Blast.Controller
{
    public class BaseCameraController
    {
        protected readonly Camera Camera;
        private readonly IGridController _gridController;

        protected BaseCameraController(Camera camera, IGridController gridController)
        {
            Camera = camera;
            _gridController = gridController;
        }

        protected void FitCameraToGrid(int rowLength, int columnLength, bool isLevelGenerator = false)
        {
            var grid = _gridController.GetGridView<IGridView>().Grid;

            var screenHeight = Camera.orthographicSize * 2f;
            var screenWidth = screenHeight * Camera.aspect;

            var gridWidth = grid.cellSize.x * columnLength;
            var gridHeight = grid.cellSize.y * rowLength;

            var cameraY = gridHeight / 2f;
            var cameraX = gridWidth / 2f;

            cameraX -= grid.cellSize.x / 2;
            cameraY -= grid.cellSize.y / 2;

            var c = Camera.transform.position;

            if (!isLevelGenerator)
            {
                SmoothMovement(new Vector3(cameraX, cameraY, c.z));
            }
            else
            {
                SmoothMovement(new Vector3(cameraX - screenWidth / 4f, cameraY, c.z));
            }
        }

        protected void SetOrto(int columnLength)
        {
            var grid = _gridController.GetGridView<IGridView>().Grid;
            var gridWidth = grid.cellSize.x * columnLength + grid.cellSize.x;
            Camera.orthographicSize = gridWidth / 2 / Camera.aspect;
        }

        private void SmoothMovement(Vector3 vec)
        {
            Camera.transform.DOMove(vec, 0.25f);
        }
    }
}