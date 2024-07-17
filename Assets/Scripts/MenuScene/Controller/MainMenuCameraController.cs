using Blast.Controller;
using DG.Tweening;
using Global.Controller;
using SC.Core.UI;
using Signals;
using UnityEngine;
using Zenject;

namespace MenuScene.Controller
{
    public class MainMenuCameraController
    {
        private Camera _camera;
        private Transform _cameraTransform;
        private Vector3 _cameraInitPosition;
        private SpriteCanvas _spriteCanvas;

        public Transform CameraTransform => _camera.transform;

        public MainMenuCameraController(SignalBus signalBus, Camera camera, SpriteCanvas spriteCanvas)
        {
            _spriteCanvas = spriteCanvas;
            _camera = camera;
            _cameraTransform = _camera.transform;
            _cameraInitPosition = _cameraTransform.position;

            signalBus.Subscribe<GameStateReaction>(OnReaction);
        }

        private void OnReaction(GameStateReaction reaction)
        {
            if (reaction.GameStatus == GameStatus.MenuInitialize)
            {
                _camera.gameObject.SetActive(true);
            }
            else if (reaction.GameStatus == GameStatus.GameInitialize)
            {
                _camera.gameObject.SetActive(false);
            }
        }

        public Vector3 GetCameraNavigationPosition(int selectedItemIndex)
        {
            float newPositionX;
            switch (selectedItemIndex)
            {
                case 0:
                    newPositionX = _cameraInitPosition.x + -2 * _spriteCanvas.ViewportWidth;
                    break;
                case 4:
                    newPositionX = _cameraInitPosition.x + 2 * _spriteCanvas.ViewportWidth;
                    break;
                case 2:
                    newPositionX = _cameraInitPosition.x;
                    break;
                default:
                    newPositionX = _cameraInitPosition.x + (selectedItemIndex - 2) * _spriteCanvas.ViewportWidth;
                    break;
            }
            return new Vector3(newPositionX, _cameraInitPosition.y,_cameraInitPosition.z);
            
        } 
    }
}