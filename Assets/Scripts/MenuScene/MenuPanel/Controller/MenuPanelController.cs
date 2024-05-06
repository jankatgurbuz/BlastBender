using System;
using Cysharp.Threading.Tasks;
using Global.Controller;
using MenuScene.MenuPanel.View;

namespace MenuScene.MenuPanel.Controller
{
    public interface IMenuPanelController
    {
        public void SmoothSlide(int index);
        public void DirectSlide(int index);
    }
    public class MenuPanelController : IMenuPanelController
    {
        private readonly MenuPanelsHandler _menuPanelsHandler;
        public MenuPanelController(MenuPanelsHandler menuPanelsHandler)
        {
            _menuPanelsHandler = menuPanelsHandler;
        }
        public void SmoothSlide(int index)
        {
            _menuPanelsHandler.SmoothSlide(index);
        }

        public void DirectSlide(int index)
        {
            _menuPanelsHandler.DirectSlide(index);
        }
    }
}
