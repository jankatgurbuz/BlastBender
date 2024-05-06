using Cysharp.Threading.Tasks;
using Global.Controller;
using UnityEngine;

namespace Blast.Controller
{
    public class GridInteractionController : BaseGridInteractionController, IStartable
    {
        public GridInteractionController(Camera camera, bool isContinuousPress, IGridController gridController) : base(
            camera, isContinuousPress, gridController)
        {
        }

        public async UniTask Start()
        {
            Receiver();
            await UniTask.Yield();
        }
    }
}