using Util.Movement.States;

namespace Util.Movement.Strategies 
{
    public class BaseMovementStrategy : IMovementStrategy
    {
        public IMoveState Current { get; set; }
        public IMoveState StartMovement { get; set; } = new StartState();
        public IMoveState FinishMovement { get; set; } = new FinishState();
        public IMoveState Shake { get; set; } = new ShakeState();
        public IMoveState CombineState { get; set; } = new CombineState();

        public void ResetAllStates()
        {
            StartMovement.ResetState();
            FinishMovement.ResetState();
            Shake.ResetState();
        }
    }
}