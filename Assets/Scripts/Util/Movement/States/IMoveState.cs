using Blast.Controller;
using Blast.View;
using BoardItems;
using UnityEngine;
using Util.Movement.Strategies;

namespace Util.Movement.States
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