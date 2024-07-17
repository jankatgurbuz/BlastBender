using System;
using System.Collections.Generic;
using Blast.Controller;
using MenuScene.Controller;
using SC.Core.Helper.Groups;
using SC.Core.UI;
using UnityEngine;
using Zenject;

namespace MenuScene.MenuCanvas.View
{
    public class NavigationItemView : MonoBehaviour
    {
        [SerializeField] private GroupSelector _groupSelector;
        [SerializeField] private HorizontalGroup _horizontalGroup;

        private List<NavigatorItem> _itemNavigator;
        private int _tempSelectedIndex = int.MaxValue;
        private bool _initFlag;
        private Vector3 _cameraNavigationPosition;

        [Inject] private MainMenuCameraController _cameraController;

        private void Awake()
        {
            _itemNavigator = new List<NavigatorItem>();
            _horizontalGroup.GetUIElementList.ForEach(x =>
            {
                _itemNavigator.Add(x.UIElement.GetComponent<NavigatorItem>());
            });
        }

        public void SelectedNavigation(int selectedItemIndex)
        {
            if (_tempSelectedIndex == selectedItemIndex) return;

            _itemNavigator.ForEach(x => { x.Passive(); });
            _groupSelector.UpdateItemScales(selectedItemIndex);
            SetCamera(selectedItemIndex);
            _itemNavigator[selectedItemIndex].Active();

            _tempSelectedIndex = selectedItemIndex;
        }

        private void SetCamera(int selectedItemIndex)
        {
            _cameraNavigationPosition = _cameraController.GetCameraNavigationPosition(selectedItemIndex);
        }

        public void InitSelected(int selectedItemIndex)
        {
            if (_initFlag) return;

            _itemNavigator[selectedItemIndex].Active();
            _cameraNavigationPosition = _cameraController.GetCameraNavigationPosition(selectedItemIndex);
            
        }

        public void ScaleUpdate(int index) 
        {
            _cameraController.CameraTransform.position =
                Vector3.Lerp(_cameraController.CameraTransform.position, _cameraNavigationPosition, 10 * Time.deltaTime);
        }

        public void ScaleAdjustmentComplete(int index)
        {
            _cameraController.CameraTransform.position = _cameraNavigationPosition;
        }
    }
}