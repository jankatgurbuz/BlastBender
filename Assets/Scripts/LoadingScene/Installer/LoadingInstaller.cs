using LoadingScene.Controller;
using LoadingScene.View;
using NaughtyAttributes;
using UnityEngine;
using Zenject;

namespace LoadingScene.Installer
{
    public class LoadingInstaller : MonoInstaller<LoadingInstaller>
    {
        [SerializeField] private LoadingView _loadingView;

        public override void InstallBindings()
        {
            Container.BindInterfacesTo<LoadingController>(). AsSingle().
                WithArguments(_loadingView).NonLazy();
        }
    }
}

