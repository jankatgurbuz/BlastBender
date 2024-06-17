using LevelGenerator.Controller;
using SC.Core.UI;
using UnityEngine;
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
                button.ClickEvent.AddListener(_saveController.SaveOnClick);
            }
        }
    }
}