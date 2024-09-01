using System.Collections.Generic;
using Blast.Controller;
using Blast.View;
using BoardItems.Border;
using SC.Core.UI;
using UnityEngine;
using Zenject;

namespace Blast.Installer
{
    public class InGameInstaller : MonoInstaller<InGameInstaller>
    {
        [SerializeField] private GridView _gridView;
        [SerializeField] private BorderProperties _borderProperties;
        [SerializeField] private List<SpriteCanvas> _canvases;
        [SerializeField] private Camera _camera;
        [SerializeField] private MovementSettings _movementSettings;

        public override void InstallBindings()
        {
            Container.Bind<BoardItemController>().To<BoardItemController>().AsSingle().NonLazy();
            Container.Bind<ISpriteCanvasController>().To<SpriteCanvasController>().AsSingle().WithArguments(_canvases);
            Container.Bind<CameraController>().To<CameraController>().AsSingle().WithArguments(_camera, _canvases)
                .NonLazy();
            Container.BindInterfacesAndSelfTo<BoardClickController>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<MovementController>().AsSingle().WithArguments(_movementSettings)
                .NonLazy();
            Container.BindInterfacesAndSelfTo<GridInteractionController>().AsSingle().WithArguments(_camera, false)
                .NonLazy();
            Container.BindInterfacesAndSelfTo<BorderProperties>().FromInstance(_borderProperties);

            Container.BindInterfacesTo<BorderController>().AsSingle().NonLazy();
            Container.BindInterfacesTo<GridController>().AsSingle().WithArguments(_gridView).NonLazy();
            Container.BindInterfacesTo<Initializer>().AsSingle();
            Container.BindInterfacesTo<InGameController>().AsSingle().NonLazy();
        }
    }
}