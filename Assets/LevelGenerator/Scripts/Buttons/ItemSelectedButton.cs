using System;
using LevelGenerator.Scripts.Controller;
using LevelGenerator.Scripts.Utility;
using SC.Core.UI;
using UnityEngine;
using Zenject;

namespace LevelGenerator.Scripts.Buttons
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