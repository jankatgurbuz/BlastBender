using BoardItems;
using Gameplay.Movement.States.BaseState;

namespace Gameplay.Movement.Strategies
{
    public interface IMovementStrategy
    {
        public IMoveState StartMovement { get; set; }
        public IMoveState FinishMovement { get; set; }
        public IMoveState Current { get; set; }
        public void ResetAllStates();
        System.Action<IMovable> AllMovementComplete { get; set; }
    }
    public interface IShakable
    {
        IMoveState Shake { get; set; }
    }

    public interface ICombinable
    {
        IMoveState CombineState { get; set; }
        void SetOffsets(int rowOffset, int columnOffset);
    }
}