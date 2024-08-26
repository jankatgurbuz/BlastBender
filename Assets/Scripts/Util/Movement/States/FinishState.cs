using System.Linq;
using Blast.Controller;
using Blast.View;
using BoardItems;
using UnityEngine;
using Util.Movement.Strategies;

namespace Util.Movement.States
{
    public class FinishState : IMoveState
    {
        private float _movementTime;

        private Vector3 _firstPosition;
        private Vector3 _targetPosition;
        private bool _isSetupComplete;
        private float _animationDuration;
        public bool AllMovementsComplete { get; set; }
        public bool IsLastMovement { get; set; }
        public bool IsFirstMovement { get; set; }

        private void Initialize(IMovable item, MovementSettings movementSettings, IGridController gridController)
        {
            if (_isSetupComplete) return;

            _firstPosition = item.TransformUtilities.GetPosition();
            _targetPosition = gridController.CellToLocal(item.Row, item.Column);

            _animationDuration = movementSettings.Shake.keys.Last().time;
            _isSetupComplete = true;
            IsLastMovement = true;
        }

        public IMoveState DoState(IMovementStrategy movementStrategy, IMovable item,
            MovementSettings movementSettings, IGridController gridController)
        {
            item.TransformUtilities.SetScale(Vector3.one);
            Initialize(item, movementSettings, gridController);
            Movement(item, movementSettings);
            return this;
        }

        private void Movement(IMovable item, MovementSettings movementSettings)
        {
            _movementTime += Time.deltaTime;

            if (_movementTime >= _animationDuration)
            {
                CompleteMovement();
                return;
            }

            ApplyFinalMovement(item, movementSettings);
        }

        private void CompleteMovement()
        {
            _movementTime = 0;
            AllMovementsComplete = true;
        }

        private void ApplyFinalMovement(IMovable item, MovementSettings movementSettings)
        {
            var evaluate = movementSettings.FinalMovementAnimationCurve.Evaluate(_movementTime);
            var clampedY = Mathf.Clamp(_firstPosition.y - evaluate, _targetPosition.y, 1000);
            var newPosition = new Vector3(_firstPosition.x, clampedY, _firstPosition.z);
            item.TransformUtilities.SetPosition(newPosition);
        }

        public void ResetState()
        {
            _isSetupComplete = false;
            AllMovementsComplete = false;
            _movementTime = 0;
        }
    }
}