// using System;
// using System.Collections;
// using System.Collections.Generic;
// using DG.Tweening;
//
// using UnityEngine;
// using UnityEngine.UI;
// using Zenject;
//
// namespace MenuScene.NavigatorPanel.View
// {
//     public interface INavigatorItem
//     {
//         public void Selected();
//         public void Unselected();
//     }
//     public class NavigationItem : MonoBehaviour, INavigatorItem
//     {
//         [SerializeField] private Transform _icon;
//         [SerializeField] private Button _button;
//         [SerializeField] private GameObject _textObject;
//         [SerializeField] private int _itemIndex;
//
//         [Inject] private readonly INavigationPanelController _navigationPanelController;
//
//         private Vector3 _selectedScale = new(1.2f, 1.2f, 1.2f);
//         private Vector3 _selectedPosition;
//         private Vector3 _currentScale;
//         private Vector3 _currentPosition;
//
//         private void Awake()
//         {
//             _currentScale = _icon.localScale;
//             _currentPosition = _icon.localPosition;
//             _selectedPosition = _icon.localPosition + new Vector3(0, 50, 0);
//             _button.onClick.AddListener(OnClick);
//         }
//
//         public void Selected()
//         {
//             _icon.DOKill();
//             _icon.localPosition = _selectedPosition;
//
//             _icon.DOScale(_selectedScale, 0.1f);
//             _textObject.SetActive(true);
//         }
//
//         public void Unselected()
//         {
//             _icon.DOKill();
//             _icon.localPosition = _currentPosition;
//             _icon.localScale = _currentScale;
//             _textObject.SetActive(false);
//         }
//
//         private void OnClick()
//         {
//             _navigationPanelController.SmoothChange(_itemIndex);
//         }
//     }
//
//
// }
