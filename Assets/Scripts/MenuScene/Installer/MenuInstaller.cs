using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Global.Controller;
using MenuScene.Controller;
using MenuScene.MenuCanvas.Controller;
using MenuScene.MenuCanvas.View;
using SC.Core.UI;
using UnityEngine;
using Zenject;

namespace MenuScene.Installer
{
    public class MenuInstaller : MonoInstaller<MenuInstaller>
    {
        [SerializeField] private List<NavigationCanvasView> navigationCanvasView;
        [SerializeField] private Camera _camera;
        [SerializeField] private SpriteCanvas _navigatorSpriteCanvas;

        public override void InstallBindings()
        {
            Container.BindInterfacesTo<NavigationCanvasController>().AsSingle().WithArguments(navigationCanvasView)
                .NonLazy();
            // Container.Bind<MainMenuCameraController>().To<MainMenuCameraController>().AsSingle()
            //     .WithArguments(_camera, _navigatorSpriteCanvas).NonLazy();

            Container.BindInterfacesAndSelfTo<MainMenuCameraController>().AsSingle().WithArguments(_camera, _navigatorSpriteCanvas).NonLazy();

            // todo:  It is bad practice to call Inject/Resolve/Instantiate before all the Installers have completed!
            Container.Resolve<GameController>().LoadNextScene().Forget();
        }
    }
}