using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Global.Controller;
using Signals;
using SpriteCanvasSystem;
using Zenject;

namespace Blast.Controller
{
    public interface ISpriteCanvasController
    {
        public void Enable();
        public void Disable();
    }
    public class SpriteCanvasController : ISpriteCanvasController
    {
        private readonly List<SpriteCanvas> _canvases;
        public SpriteCanvasController(SignalBus signalBus,List<SpriteCanvas> canvases)
        {
            _canvases = canvases;
            signalBus.Subscribe<GameStateReaction>(OnReaction);
        }

        private void OnReaction(GameStateReaction reaction)
        {
            if (reaction.GameStatus == GameStatus.GameInitialize)
            {
                AdjustUIAfterFrame().Forget();
            }
        }

        public async UniTask AdjustUIAfterFrame()
        {
            await UniTask.Yield();
            _canvases.ForEach(x => x.Adjust());
        }

        public void Disable() => _canvases.ForEach(x => x.DisableUI());
        public void Enable() => _canvases.ForEach(x => x.EnableUI());
    }
}
