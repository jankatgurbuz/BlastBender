using Cysharp.Threading.Tasks;
using Global.Controller;
using UnityEngine;
using Util.DebugTools;

namespace Blast.Controller
{
    public class DebugController : IStartable
    {
        private bool _enableDebug;
        private float _gameSpeed = 1;
        private CameraController _cameraController;

        public DebugController(bool enableDebug, CameraController cameraController)
        {
            _enableDebug = enableDebug;
            _cameraController = cameraController;
        }

        public UniTask Start()
        {
            if (_enableDebug)
            {
                ReadKey();
            }

            return UniTask.CompletedTask;
        }

        private async void ReadKey()
        {
            while (true)
            {
                AdjustGameSpeed();
                await UniTask.Yield(PlayerLoopTiming.Update);
            }
        }

        private void AdjustGameSpeed()
        {
            var isLeftShiftPressed = Input.GetKey(KeyCode.LeftShift);
            var isLeftCommandPressed = Input.GetKey(KeyCode.LeftCommand);
            var isLeftOptionPressed = Input.GetKey(KeyCode.LeftAlt);
            
            if (isLeftShiftPressed && isLeftCommandPressed)
            {
                if (Input.GetKeyDown(KeyCode.Equals))
                {
                    _gameSpeed += isLeftOptionPressed ? 0.01f : 0.1f;
                    UpdateGameSpeed();
                }
                else if (Input.GetKeyDown(KeyCode.Minus))
                {
                    _gameSpeed -= isLeftOptionPressed ? 0.01f : 0.1f;
                    UpdateGameSpeed();
                }
            }
            
            void UpdateGameSpeed()
            {
                _gameSpeed = Mathf.Clamp(_gameSpeed, 0f, 10f);
                DebugText.GetText("TimeScale: " + _gameSpeed.ToString("F2"), _cameraController);
                Time.timeScale = _gameSpeed;
            }
        }
    }
}