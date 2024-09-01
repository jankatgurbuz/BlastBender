using LoadingScene.Controller;
using LoadingScene.View;
using UnityEngine;
using Zenject;

namespace LoadingScene.Installer
{
    public class LoadingInstaller : MonoInstaller<LoadingInstaller>
    {
        [SerializeField] private LoadingView _loadingView;

        public override void InstallBindings()
        {
            Container.Bind<LoadingController>().AsSingle().WithArguments(_loadingView).NonLazy();
            Container.BindInterfacesTo<LoadingSceneInitializer>().AsSingle().NonLazy();
        }
    }
}