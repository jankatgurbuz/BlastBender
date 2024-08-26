using Blast.Controller;
using Blast.View;
using BoardItems;
using Gameplay.Movement.Strategies;
using UnityEngine;

namespace Gameplay.Movement.States.BaseState
{
    public interface IMoveState
    {
        bool AllMovementsComplete { get; set; }
        bool IsLastMovement { get; set; }
        bool IsFirstMovement { get; set; }

        IMoveState DoState(IMovementStrategy movementStrategy, IMovable item,
            MovementSettings movementSettings, IGridController gridController);

        void ResetState();
    }

    public interface IPositionSetter
    {
        public void SetTargetPosition(Vector3 position);
    }
}