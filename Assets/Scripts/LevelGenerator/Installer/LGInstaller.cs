using System.Collections.Generic;
using Blast.Controller;
using BoardItems.Border;
using LevelGenerator.Controller;
using LevelGenerator.GridSystem.View;
using SpriteCanvasSystem;
using UnityEngine;
using Zenject;

namespace LevelGenerator.Installers
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
            Container.Bind<LGBoardItemController>().To<LGBoardItemController>().AsSingle().NonLazy();
            Container.Bind<ISpriteCanvasController>().To<SpriteCanvasController>().AsSingle().WithArguments(_canvases);
            Container.Bind(typeof(LGBorderController)).To<LGBorderController>().AsSingle().NonLazy();
            Container.Bind<ILGGridInteractionController>().To<LGGridInteractionController>().AsSingle().WithArguments(_camera, true)
                .NonLazy();

            Container.Bind<BorderProperties>().FromInstance(_borderProperties);
            Container.Bind(typeof(ILGGridController), typeof(IGridController)).To<LGGridController>().AsSingle()
                .WithArguments(_gridView);
            Container.BindInterfacesAndSelfTo<LGInitialize>().AsSingle();
        }
    }
}