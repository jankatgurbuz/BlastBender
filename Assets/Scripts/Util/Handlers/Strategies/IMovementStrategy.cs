using DG.Tweening;
using UnityEngine;

namespace Util.Handlers.Strategies
{
    public interface IMovementStrategy
    {
        Sequence FinalMovement(Transform transform, Vector3 currentScale);
        Sequence StartMovement(Transform transform);
        Sequence Shake(Transform transform);
    }
}
