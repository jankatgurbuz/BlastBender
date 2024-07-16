using System;
using System.Collections.Generic;
using SC.Core.Helper.Groups;
using SC.Core.UI;
using UnityEngine;

namespace MenuScene.MenuCanvas.View
{
    public class NavigationItemView : MonoBehaviour
    {
        [SerializeField] private GroupSelector _groupSelector;
        [SerializeField] private HorizontalGroup _horizontalGroup;
        private List<NavigatorItem> _itemNavigator;
        private int _tempSelectedIndex = int.MaxValue;
        private bool _initFlag;

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
           
            _itemNavigator.ForEach(x => { x.Passive();});
            _groupSelector.UpdateItemScales(selectedItemIndex);
            _itemNavigator[selectedItemIndex].Active();

            _tempSelectedIndex = selectedItemIndex;
        }

        public void InitSelected(int selectedItemIndex)
        {
            if (_initFlag) return;
            
            _itemNavigator[selectedItemIndex].Active();
            _initFlag = true;
        }
    }
}
