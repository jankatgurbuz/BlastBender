using Cysharp.Threading.Tasks;
using UnityEngine;
using Util.Movement.States;

namespace Util.Movement.Strategies
{
    public interface IMovementStrategy
    {
        bool IsPlayShake { get; set; }
        bool IsPlayStartMovement { get; set; }
        bool IsPlayFinalMovement { get; set; }

        UniTask Shake2(Transform transform);
        UniTask StartMovement2(Transform transform);
        UniTask FinalMovement2(Transform transform, Vector3 currentScale);

        bool Kill { set; }
        public IMoveState StartMovement { get; set; }
        public IMoveState FinishMovement { get; set; }
        public IMoveState Current { get; set; }

        public void Restart();
    }
}