using SC.Core.UI;
using UnityEngine;

namespace MenuScene.MenuCanvas.View
{
    public class NavigatorItem : MonoBehaviour
    {
        [SerializeField] private GameObject _textMeshProObj;
        [SerializeField] private Transform _iconObj;
        [SerializeField] private UIElement _uiElement;


        private void Awake()
        {
            Passive();
        }
        public void Passive()
        {
            _uiElement.UIElementProperties.IgnoreYPosition = false;
            _textMeshProObj.SetActive(false);
        }

        public void Active()
        {
            _uiElement.UIElementProperties.IgnoreYPosition = true;
            _iconObj.position += new Vector3(0, 0.1f, 0);
            _textMeshProObj.SetActive(true);
        }
    }
}