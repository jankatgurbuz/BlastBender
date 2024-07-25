using Blast.View;
using Cysharp.Threading.Tasks;
using UnityEngine;


namespace Blast.Controller
{
    public class BaseCameraController
    {
        public readonly Camera Camera;
        private readonly IGridController _gridController;

        protected BaseCameraController(Camera camera, IGridController gridController)
        {
            Camera = camera;
            _gridController = gridController;
        }

        protected async void FitCameraToGrid(int rowLength, int columnLength, bool isLevelGenerator = false)
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
                Camera.transform.position = new Vector3(cameraX, cameraY, c.z);
            }
            else
            {
                await MoveCameraOverTime(Camera.transform, new Vector3(cameraX - screenWidth / 4f, cameraY, c.z), 0.25f);
            }
        }
        
        private async UniTask MoveCameraOverTime(Transform target, Vector3 endPosition, float duration)
        {
            var startPosition = target.position;
            float elapsedTime = 0;

            while (elapsedTime < duration)
            {
                target.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                await UniTask.Yield(PlayerLoopTiming.Update);
            }

            target.position = endPosition;
        }

        protected void SetOrto(int columnLength)
        {
            var grid = _gridController.GetGridView<IGridView>().Grid;
            var gridWidth = grid.cellSize.x * columnLength + grid.cellSize.x;
            Camera.orthographicSize = gridWidth / 2 / Camera.aspect;
        }
    }
}