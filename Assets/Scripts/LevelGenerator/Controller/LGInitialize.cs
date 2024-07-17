using Zenject;

namespace LevelGenerator.Controller
{
    public interface ILGStart
    {
        public void Start();
    }

    public class LGInitialize : IInitializable
    {
        private readonly DiContainer _container;

        public LGInitialize(DiContainer container)
        {
            _container = container;
        }

        public void Initialize()
        {
            // _container.Resolve<LGBoardItemController>().Start();
            var levelGeneratorController = _container.Resolve<ILevelGeneratorController>();
            levelGeneratorController.Start();
            
            _container.Resolve<LGBorderController>().Start();
            _container.Resolve<LGPointIndicatorController>().Start();
          
            
            _container.Resolve<ILGGridInteractionController>().Start();

            levelGeneratorController.Initialize();
        }
    }
}