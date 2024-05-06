using BoardItems;
using Util.Handlers.Strategies;

namespace Util.Handlers.Visitors
{
    public class MovementVisitor
    {
        private readonly IMoveable _itemView;
        private readonly IMovementStrategy _movementStrategy;
        
        public static readonly MovementVisitor Empty = new(false);
        public readonly bool IsMoveable;
        public float MovementTime;
        
        private MovementVisitor(bool isEmpty)
        {
            IsMoveable = isEmpty;
        }
        public MovementVisitor(IMoveable itemView,IMovementStrategy movementStrategy)
        {
            IsMoveable = true;
            _itemView = itemView;
            _movementStrategy = movementStrategy;
        }
        public void FinalizeMovementWithBounce()
        {
            _itemView?.FinalizeMovementWithBounce(_movementStrategy);
        }
        public void StartMovement()
        {
            _itemView?.StartMovement(_movementStrategy);
        }

        public void Shake()
        {
            _itemView?.Shake(_movementStrategy);
        }
    }
}