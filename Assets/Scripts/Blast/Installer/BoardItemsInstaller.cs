using System;
using Blast.Factory;
using BoardItems;
using BoardItems.Bead;
using BoardItems.Obstacles;
using Gameplay.Movement.Strategies;
using Zenject;

namespace Blast.Installer
{
    public class BoardItemsInstaller : MonoInstaller<InGameInstaller>
    {
        public override void InstallBindings()
        {
            
            Container.Bind<IMovementStrategy>().To<BeadMovementStrategy>().AsTransient().WhenInjectedInto<Bead>();
            Container.Bind<IMovementStrategy>().To<DuckMovementStrategy>().AsTransient().WhenInjectedInto<Duck>();

            Container.BindFactory<Type, object[], IBoardItem, BoardItemFactory>()
                .FromFactory<BoardItemFactory>().NonLazy();
        }
    }
}