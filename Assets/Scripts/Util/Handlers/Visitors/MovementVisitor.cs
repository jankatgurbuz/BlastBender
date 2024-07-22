using BoardItems;
using UnityEngine;
using Util.Handlers.Strategies;
using Util.Movement.Strategies;
using Util.Pool.Bead;

namespace Util.Handlers.Visitors
{
    public class MovementVisitor
    {
        public static readonly MovementVisitor Empty = new(false);
        public readonly bool IsMovementSupported;
        public IMovementStrategy MovementStrategy { get; }
        public bool IsFinish = false;

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
        // private readonly IMoveable _itemView;
        // private readonly IMovementStrategy _movementStrategy;
        //
        // public static readonly MovementVisitor Empty = new(false);
        // public readonly bool IsMoveable;
        // public float MovementTime;
        //
        // private MovementVisitor(bool isEmpty)
        // {
        //     IsMoveable = isEmpty;
        // }
        // public MovementVisitor(IMoveable itemView,IMovementStrategy movementStrategy)
        // {
        //     IsMoveable = true;
        //     _itemView = itemView;
        //     _movementStrategy = movementStrategy;
        // }
        // public void FinalizeMovementWithBounce()
        // {
        //     _itemView?.FinalizeMovementWithBounce(_movementStrategy);
        // }
        // public void StartMovement()
        // {
        //     _itemView?.StartMovement(_movementStrategy);
        // }
        //
        // public void Shake()
        // {
        //     _itemView?.Shake(_movementStrategy);
        // }
        
    }
}