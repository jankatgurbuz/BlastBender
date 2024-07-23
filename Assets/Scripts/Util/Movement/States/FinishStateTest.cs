using System.Linq;
using Blast.Controller;
using Blast.View;
using BoardItems;
using UnityEngine;
using Util.Movement.Strategies;

namespace Util.Movement.States
{
    public class FinishStateTest : IMoveState
    {
        private float _movementTime;
      
        private Vector3 _firstPosition;
        private Vector3 _targetPosition;
        private bool _init;
        
        public bool AllMovementsComplete { get; set; }

        public IMoveState DoState(IMovementStrategy movementStrategy, IBoardItem item,
            MovementSettings movementSettings,
            IGridController gridController)
        {
            item.MovementVisitor.MoveableItem.GetTransform().transform.localScale = Vector3.one;
            Initialize(movementStrategy,item,movementSettings,gridController);
            Movement(movementStrategy, item, movementSettings, gridController);
            return this;
        }

        private void Movement(IMovementStrategy movementStrategy, IBoardItem item, MovementSettings movementSettings,
            IGridController gridController)
        {
            _movementTime += Time.deltaTime;
            var animationDuration = movementSettings.FinalMovementAnimationCurve.keys.Last().time;

            if (_movementTime >= animationDuration)
            {
                _movementTime = 0;
                AllMovementsComplete = true;
                return;
            }

            var evaluate = movementSettings.FinalMovementAnimationCurve.Evaluate(_movementTime);
            var clampedY = Mathf.Clamp(_firstPosition.y - evaluate, _targetPosition.y, 1000);
            var newPosition = new Vector3(_firstPosition.x, clampedY, _firstPosition.z);
            item.SetPosition(newPosition);
        }

        private void Initialize(IMovementStrategy movementStrategy, IBoardItem item, MovementSettings movementSettings,
            IGridController gridController)
        {
            if (!_init)
            {
                _firstPosition = item.GetPosition();
                _init = true;
            }
        }

        public void ResetState()
        {
            _init = false;
            AllMovementsComplete = false;
            _movementTime = 0;
        }
    }
}