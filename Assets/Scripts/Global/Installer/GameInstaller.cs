using Global.Controller;
using Global.View;
using Signals;
using UnityEngine;
using Zenject;

namespace Global.Installer
{
    public class GameInstaller : MonoInstaller<GameInstaller>
    {
        [SerializeField] private LayersProperties _layersProperties;

        public override void InstallBindings()
        {
            SignalBusInstaller.Install(Container);
            Container.BindInterfacesAndSelfTo<GameController>().AsSingle().NonLazy();
            Container.Bind<LayersController>().To<LayersController>().AsSingle().WithArguments(_layersProperties)
                .NonLazy();
            Container.DeclareSignal<GameStateReaction>().OptionalSubscriber();
        }
    }
}