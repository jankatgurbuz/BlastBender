using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Global.Controller;
using MenuScene.MenuCanvas.Controller;
using MenuScene.MenuCanvas.View;
using UnityEngine;
using Zenject;

namespace MenuScene.Installer
{
    public class MenuInstaller : MonoInstaller<MenuInstaller>
    {
        [SerializeField] private List<NavigationCanvasView> navigationCanvasView;

        public override void InstallBindings()
        {
            Container.BindInterfacesTo<NavigationCanvasController>().AsSingle().WithArguments(navigationCanvasView)
                .NonLazy();
            
            // todo:  It is bad practice to call Inject/Resolve/Instantiate before all the Installers have completed!
            Container.Resolve<GameController>().LoadNextScene().Forget();
        }
    }
}