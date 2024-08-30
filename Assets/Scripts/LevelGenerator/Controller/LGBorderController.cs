using Blast.Controller;
using BoardItems.Border;
using Zenject;

namespace LevelGenerator.Controller
{
    public class LGBorderController : BaseBorderController, ILGStart
    {
        private readonly ILevelGeneratorController _levelGeneratorController;

        public LGBorderController(SignalBus signalBus, BorderProperties borderProperties,
            LGGridController gridController, ILevelGeneratorController levelGeneratorController) : base(
            borderProperties, gridController)
        {
            _levelGeneratorController = levelGeneratorController;
        }

        public void Start()
        {
            _levelGeneratorController.OnChangeState += CreateBorderMatrix;
        }

        private void CreateBorderMatrix()
        {
            CreateBorderMatrix(_levelGeneratorController.RowLength, _levelGeneratorController.ColumnLength,
                _levelGeneratorController.LevelData);
        }
    }
}