using UnityEngine;
using System;
using LevelGenerator.Controller;
using LevelGenerator.Utility;
using SC.Core.UI;
using Zenject;

namespace LevelGenerator.Buttons
{
    public class ItemSelectedButton : MonoBehaviour
    {
        [HideInInspector] public string SelectedType;
        public BoardItems.ItemColors ItemColors;
        public TaskLocation TaskLocation = TaskLocation.Board;

        [Inject] private readonly ILevelGeneratorController _levelGeneratorController;

        private UIButton _button;

        private void Awake()
        {
            _button = GetComponent<UIButton>();
            _button.ClickEvent.AddListener(OnClick);
        }

        private void OnClick()
        {
            _levelGeneratorController.SelectedType = Type.GetType(SelectedType);
            _levelGeneratorController.ItemColors = ItemColors;
            _levelGeneratorController.TaskLocation = TaskLocation;
        }
    }
}