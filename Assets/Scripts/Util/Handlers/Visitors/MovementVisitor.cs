using BoardItems;
using Util.Movement.Strategies;

namespace Util.Handlers.Visitors
{
    public class MovementVisitor
    {
        public static readonly MovementVisitor Empty = new(false);
        public bool IsFinish = false;
        public bool IsMovementSupported { get; }
        public IMovementStrategy MovementStrategy { get; set; }
        public IMoveable MoveableItem { get; set; }

        public MovementVisitor(IMovementStrategy movementStrategy)
        {
            IsMovementSupported = true;
            MovementStrategy = movementStrategy;
        }

        private MovementVisitor(bool isEmpty)
        {
            IsMovementSupported = isEmpty;
        }
    }
}