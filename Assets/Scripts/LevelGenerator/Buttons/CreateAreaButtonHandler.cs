using LevelGenerator.Controller;
using UnityEngine;
using Zenject;

namespace LevelGenerator.Buttons
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
