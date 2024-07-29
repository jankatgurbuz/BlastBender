using System.Collections.Generic;
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
            Container.Bind<NavigationCanvasController>().To<NavigationCanvasController>().AsSingle()
                .WithArguments(navigationCanvasView).NonLazy();

            Container.Bind<MainMenuCameraController>().To<MainMenuCameraController>().AsSingle()
                .WithArguments(_camera, _navigatorSpriteCanvas).NonLazy();

            Container.BindInterfacesAndSelfTo<MainMenuInitializer>().AsSingle().NonLazy();
        }
    }
}