using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Util.Handlers.Strategies
{
    public interface IMovementStrategy
    {
        bool IsPlayShake { get; set; }
        bool IsPlayStartMovement { get; set; }
        bool IsPlayFinalMovement { get; set; }

        UniTask Shake2(Transform transform);
        UniTask StartMovement2(Transform transform);
        UniTask FinalMovement2(Transform transform, Vector3 currentScale);

        bool Kill {  set; }
    }
}