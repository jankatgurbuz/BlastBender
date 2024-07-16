using SC.Core.Helper.Groups;
using SC.Core.UI;
using UnityEngine;

namespace MenuScene.MenuCanvas.View
{
    public class NavigationCanvasView : MonoBehaviour
    {
        [SerializeField] private SpriteCanvas _spriteCanvas;
        [SerializeField] private GroupSelector _groupSelector;
        public SpriteCanvas SpriteCanvas => _spriteCanvas;

        public void Hide()
        {
            _spriteCanvas.HideAllUIs();
        }

        public void Show()
        {
            _spriteCanvas.ShowAllUIs();
        }

        public void SelectedNavigation(int selectedItemIndex)
        {
            _groupSelector.UpdateItemScales(selectedItemIndex);
        }
    }
}