using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MenuScene.MenuPanel.View
{
    public class MenuPanel : MonoBehaviour
    {
        [SerializeField] private RectTransform _rectTransform;
        public RectTransform RectTransform { get => _rectTransform; }
    }
}
