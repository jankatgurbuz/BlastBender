using Cysharp.Threading.Tasks;
using Global.Controller;
using MenuScene.MenuCanvas.Controller;
using MenuScene.MenuPanel.Controller;
using MenuScene.MenuPanel.View;
using MenuScene.NavigatorPanel.View;
using UnityEngine;
using Zenject;

namespace MenuScene.Installer
{
    public class MenuInstaller : MonoInstaller<MenuInstaller>
    {
        [SerializeField] private NavigationView _navigationView;
        [SerializeField] private MenuPanelsHandler _menuPanelsHandler;
        [SerializeField] private MenuCanvas.View.MenuCanvas _menuCanvas;

        public override void InstallBindings()
        {
            Container.BindInterfacesTo<NavigationPanelController>().AsSingle().
                WithArguments(_navigationView).NonLazy();

            Container.BindInterfacesTo<MenuPanelController>().AsSingle().
              WithArguments(_menuPanelsHandler).NonLazy();

            Container.BindInterfacesTo<MenuCanvasController>().AsSingle().
            WithArguments(_menuCanvas).NonLazy();

            // todo:  It is bad practice to call Inject/Resolve/Instantiate before all the Installers have completed!
            Container.Resolve<GameController>().LoadNextScene().Forget(); 
        }
    }
}
