using System.Collections;
using System.Collections.Generic;
using LevelGenerator.Controller;
using SpriteCanvasSystem;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace LevelGenerator.Buttons
{
    public class SaveButtonHandler : MonoBehaviour
    {
        private ILGSaveController _saveController;

        [Inject]
        public void Construct(ILGSaveController saveController)
        {
            _saveController = saveController;
        }

        private void Start()
        {
            if (TryGetComponent<UIButton>(out var button))
            {
                button.OnClick.AddListener(_saveController.SaveOnClick);
            }
        }
    }
}
