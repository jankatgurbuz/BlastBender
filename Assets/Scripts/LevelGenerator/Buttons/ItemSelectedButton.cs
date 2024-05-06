using UnityEngine;
using NaughtyAttributes;
using SpriteCanvasSystem;
using System;
using LevelGenerator.Controller;
using Zenject;

namespace LevelGenerator.Buttons
{
    public class ItemSelectedButton : MonoBehaviour
    {
        [HideInInspector] public string SelectedType;
        public BoardItems.ItemColors ItemColors;

        [Inject] private readonly ILevelGeneratorController _levelGeneratorController;

        private UIButton _button;
        private void Awake()
        {
            _button = GetComponent<UIButton>();
            _button.OnClick.AddListener(OnClick);
        }

        private void OnClick(Vector3 arg0)
        {
            _levelGeneratorController.SelectedType = Type.GetType(SelectedType);
            _levelGeneratorController.ItemColors = ItemColors;
        }
    }
}