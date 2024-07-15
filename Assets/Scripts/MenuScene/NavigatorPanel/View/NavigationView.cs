using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.UIElements;
using System;

namespace MenuScene.NavigatorPanel.View
{
    public class NavigationView : MonoBehaviour
    {
        [SerializeField] private RectTransform _navigationSubPanel;
        [SerializeField] private List<NavigationItemProperty> _navigationItemList;
        [SerializeField] private RectTransform _indicator;

        private List<float> _unselectedPositions;
        private List<float> _selectedPositions;
        private List<NavigationItemInfo> _navigationInfoList;

        public class NavigationItemInfo
        {
            public int SelectedIndex;
            public List<int> PositionList;
        }
        [System.Serializable]
        public class NavigationItemProperty
        {
            public RectTransform RectTransform;
            public NavigationItem Item;
        }

        [NaughtyAttributes.Button]
        private void Start()
        {
            _unselectedPositions = CreatePositions(6);
            _selectedPositions = CreatePositions(5, 6);
            CreateNavigationInfoList();
            ChangeNavigation(2);
            ArrangeIndicatorSize();
        }

        private void ArrangeIndicatorSize()
        {
            var xsize = _navigationSubPanel.rect.width / 6f * 2;
            _indicator.sizeDelta = new Vector2(xsize, 0);
        }

        private List<float> CreatePositions(int piece, float? referenceSize = null)
        {
            var width = _navigationSubPanel.rect.width;

            if (referenceSize != null)
                width -= (width / (float)referenceSize);

            var widthOfpiece = width / piece;

            var tempList = new List<float>();
            for (int i = 0; i < piece; i++)
            {
                var x = -(width * 0.5f) + (widthOfpiece * 0.5f) + (widthOfpiece * i);
                tempList.Add(x);
            }

            return tempList;
        }

        private void CreateNavigationInfoList()
        {
            _navigationInfoList = new List<NavigationItemInfo>()
            {
                new NavigationItemInfo()
                { SelectedIndex = 0, PositionList = new List<int> { 2, 3, 4, 5 } },
                new NavigationItemInfo()
                { SelectedIndex = 1, PositionList = new List<int> { 0, 3, 4, 5 } },
                new NavigationItemInfo()
                { SelectedIndex = 2, PositionList = new List<int> { 0, 1, 4, 5 } },
                new NavigationItemInfo()
                { SelectedIndex = 3, PositionList = new List<int> { 0, 1, 2, 5 } },
                new NavigationItemInfo()
                { SelectedIndex = 4, PositionList = new List<int> { 0, 1, 2, 3 } }
            };
        }

        public void ChangeNavigation(int selectedIndex)
        {
            int itemCounter = 0;
            var info = _navigationInfoList.Find((x) => x.SelectedIndex == selectedIndex);

            for (int i = 0; i < _navigationItemList.Count; i++)
            {
                if (i == selectedIndex)
                {
                    Move(_selectedPositions, selectedIndex, i, true);
                    SetNavigationItemsSize(i, true);
                    _navigationItemList[i].Item.Selected();
                    continue;
                }

                var positionIndex = info.PositionList[itemCounter];
                Move(_unselectedPositions, positionIndex, i);
                SetNavigationItemsSize(i, false);
                _navigationItemList[i].Item.Unselected();

                itemCounter++;
            }
        }
        public void SetNavigationItemsSize(int index, bool isSelectable)
        {
            var piece = 6f;
            var width = _navigationSubPanel.rect.width / piece;
            var item = _navigationItemList[index].RectTransform;
            var size = new Vector2(width * (isSelectable ? 2 : 1), item.sizeDelta.y);
            item.DOSizeDelta(size, 0.1f);
        }
        public void Move(List<float> positionsList, int positionIndex, int navigationIndex, bool moveIndicator = false)
        {
            var transform = _navigationItemList[navigationIndex].RectTransform;
            var position = new Vector2(positionsList[positionIndex], transform.anchoredPosition.y);
            transform.DOAnchorPos(position, 0.1f);

            if (moveIndicator)
                _indicator.DOAnchorPos(position, 0.1f);
            
        }
    }
}
