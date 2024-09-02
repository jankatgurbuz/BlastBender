using LevelGenerator.Scripts.Controller;
using UnityEngine;
using Zenject;

namespace LevelGenerator.Scripts.Buttons
{
    public class CreateAreaButtonHandler : MonoBehaviour
    {
        [Inject]private LGCameraController _cameraController;
        public void OnClick()
        {
            _cameraController.CreateArea();
        }
    }
}
