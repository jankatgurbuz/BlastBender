using Util.Movement.States;

namespace Util.Movement.Strategies
{
    public interface IMovementStrategy
    {
        public IMoveState StartMovement { get; set; }
        public IMoveState FinishMovement { get; set; }
        public IMoveState Current { get; set; }
        public IMoveState Shake { get; set; }
        public void ResetAllStates();
    }
}