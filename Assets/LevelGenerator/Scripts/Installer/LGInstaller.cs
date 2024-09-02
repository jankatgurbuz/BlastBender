using System.Collections.Generic;
using BoardItems.Border;
using LevelGenerator.Scripts.Controller;
using LevelGenerator.Scripts.View;
using SC.Core.UI;
using UnityEngine;
using Zenject;

namespace LevelGenerator.Scripts.Installer
{
    public class LGInstaller : MonoInstaller<LGInstaller>
    {
        [SerializeField] private LGGridView _gridView;
        [SerializeField] private BorderProperties _borderProperties;
        [SerializeField] private List<SpriteCanvas> _canvases;
        [SerializeField] private Camera _camera;

        public override void InstallBindings()
        {
            Container.Bind<ILevelGeneratorController>().To<LevelGeneratorController>().AsSingle().NonLazy();
            Container.Bind<ILGSaveController>().To<LGSaveController>().AsSingle().NonLazy();
            Container.Bind<LGCameraController>().To<LGCameraController>().AsSingle().WithArguments(_camera).NonLazy();

            Container.Bind<ILGGridInteractionController>().To<LGGridInteractionController>().AsSingle()
                .WithArguments(_camera, true)
                .NonLazy();
            Container.Bind<BorderProperties>().FromInstance(_borderProperties);

            Container.BindInterfacesAndSelfTo<LGSpriteCanvasController>().AsSingle().WithArguments(_canvases);
            Container.BindInterfacesAndSelfTo<LGInitialize>().AsSingle();
            Container.Bind<LGSpawnerController>().To<LGSpawnerController>().AsSingle().NonLazy();

            Container.BindInterfacesAndSelfTo<LGPointIndicatorController>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<LGBorderController>().AsSingle().NonLazy();
            Container.Bind<LGGridController>().To<LGGridController>().AsSingle().WithArguments(_gridView);
        }
    }
}