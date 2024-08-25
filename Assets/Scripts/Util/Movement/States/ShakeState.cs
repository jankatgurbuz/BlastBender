using System.Linq;
using Blast.Controller;
using Blast.View;
using BoardItems;
using UnityEngine;
using Util.Movement.Strategies;

namespace Util.Movement.States
{
    public class ShakeState : IMoveState
    {
        private bool _isSetupComplete;
        private float _movementTime;
        private float _animationDuration;
        public bool AllMovementsComplete { get; set; }

        private void Initialize(IMovable item, MovementSettings movementSettings)
        {
            if (_isSetupComplete) return;

            _animationDuration = movementSettings.Shake.keys.Last().time;
            item.TransformUtilities.SetRotation(Vector3.zero);
            _isSetupComplete = true;
        }

        public IMoveState DoState(IMovementStrategy movementStrategy, IMovable item,
            MovementSettings movementSettings, IGridController gridController)
        {
            item.TransformUtilities.SetScale(Vector3.one);
            Initialize(item, movementSettings);
            Movement(item, movementSettings);
            return this;
        }

        private void Movement(IMovable item, MovementSettings movementSettings)
        {
            _movementTime += Time.deltaTime;

            if (_movementTime >= _animationDuration)
            {
                CompleteMovement(item);
                return;
            }

            ApplyShake(item, movementSettings);
        }

        private void ApplyShake(IMovable item, MovementSettings movementSettings)
        {
            var evaluate = movementSettings.Shake.Evaluate(_movementTime);
            var newRotation = new Vector3(0, 0, evaluate);
            item.TransformUtilities.SetRotation(newRotation);
        }

        private void CompleteMovement(IMovable item)
        {
            item.TransformUtilities.SetRotation(Vector3.zero);
            AllMovementsComplete = true;
            _movementTime = 0;
        }

        public void ResetState()
        {
            AllMovementsComplete = false;
            _isSetupComplete = false;
            _movementTime = 0;
        }
    }
}