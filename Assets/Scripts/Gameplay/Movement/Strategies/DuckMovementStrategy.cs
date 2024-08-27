using System;
using BoardItems;
using Gameplay.Movement.States.BaseState;
using Gameplay.Movement.States.DuckState;
using Gameplay.Movement.Strategies;

public class DuckMovementStrategy : IMovementStrategy, IShakable
{
    public IMoveState StartMovement { get; set; } = new StartState();
    public IMoveState FinishMovement { get; set; } = new FinishState();
    public IMoveState Current { get; set; }

    public void ResetAllStates()
    {
        StartMovement.ResetState();
        FinishMovement.ResetState();
        Shake.ResetState();
    }

    public Action<IMovable> AllMovementComplete { get; set; }
    public IMoveState Shake { get; set; } = new DcukShakeState();
}