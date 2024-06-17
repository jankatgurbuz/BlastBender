using System.Collections.Generic;
using Blast.Controller;
using Cysharp.Threading.Tasks;
using SC.Core.UI;
using Zenject;

namespace LevelGenerator.Controller
{
    public class LGSpriteCanvasController : SpriteCanvasController, ILateTickable
    {
        private DiContainer _container;

        public LGSpriteCanvasController(SignalBus signalBus, List<SpriteCanvas> canvases,
            DiContainer container) : base(signalBus, canvases)
        {
            _container = container;
        }
        public void LateTick()
        {
            Adjust();
        }
    }
}