using Blast.Controller;
using Blast.View;
using Zenject;

namespace LevelGenerator.Controller
{
    public class LGGridController : BaseGridController
    {
        public LGGridController(DiContainer container, IGridView gridView) : base(gridView)
        {
        }
    }
}