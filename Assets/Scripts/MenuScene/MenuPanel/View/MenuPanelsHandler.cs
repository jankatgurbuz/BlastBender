using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace MenuScene.MenuPanel.View
{
    public class MenuPanelsHandler : MonoBehaviour
    {
        [SerializeField] private List<MenuPanel> _menuPanels;
        [SerializeField] private MenuPanel _referencePanel;
        [SerializeField] private RectTransform _parentMenuPanel;

        private readonly List<int> _slideOffsetList = new();

        private void Start()
        {
            var width = _referencePanel.RectTransform.rect.width;
            var slideOffset = Mathf.FloorToInt(_menuPanels.Count / 2f);

            for (int i = 0; i < _menuPanels.Count; i++)
            {
                var rt = _menuPanels[i].RectTransform;
                rt.anchoredPosition = new Vector2(
                    width * (i - slideOffset), rt.anchoredPosition.y);

                _slideOffsetList.Add(i - slideOffset);
            }
        }

        public void SmoothSlide(int index)
        {
            var width = _referencePanel.RectTransform.rect.width;
            _parentMenuPanel.DOAnchorPos(new Vector2(width * -_slideOffsetList[index], 0), 0.25f);
        }

        public void DirectSlide(int index)
        {
            var width = _referencePanel.RectTransform.rect.width;
            _parentMenuPanel.anchoredPosition = new Vector2(width * -_slideOffsetList[index], 0);
        }
    }
}
