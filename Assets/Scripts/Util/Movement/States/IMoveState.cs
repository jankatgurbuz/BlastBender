using Blast.Controller;
using Blast.View;
using BoardItems;
using Util.Movement.Strategies;

namespace Util.Movement.States
{
    public interface IMoveState
    {
        bool AllMovementsComplete { get; set; }

        IMoveState DoState(IMovementStrategy movementStrategy, IMovable item,
            MovementSettings movementSettings, IGridController gridController);

        void ResetState();
    }
}