using System;
using BoardItems;
using Gameplay.Movement.States.BaseState;

namespace Gameplay.Movement.Strategies
{
    public class BeadMovementStrategy : IMovementStrategy, IShakable, ICombinable
    {
        public IMoveState Current { get; set; }
        public IMoveState StartMovement { get; set; } = new StartState();
        public IMoveState FinishMovement { get; set; } = new FinishState();
        public IMoveState Shake { get; set; } = new ShakeState();
        public IMoveState CombineState { get; set; } = new CombineState();
        public Action<IMovable> AllMovementComplete { get; set; }

        public void SetOffsets(int rowOffset, int columnOffset)
        {
            if (CombineState is CombineState cs)
            {
                cs.SetOffsets(rowOffset, columnOffset);
            }
        }

        public void ResetAllStates()
        {
            StartMovement.ResetState();
            FinishMovement.ResetState();
            Shake.ResetState();
            CombineState.ResetState();
        }
    }
}