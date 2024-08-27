using System.Linq;
using Blast.Controller;
using Blast.View;
using BoardItems;
using Gameplay.Movement.States.BaseState;
using Gameplay.Movement.Strategies;
using UnityEngine;

namespace Gameplay.Movement.States.DuckState
{
    public class DcukShakeState : IMoveState
    {
        private bool _isSetupComplete;
        private float _movementTime;
        private float _animationDuration;
        public bool AllMovementsComplete { get; set; }
        public bool IsLastMovement { get; set; }
        public bool IsFirstMovement { get; set; }

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
            var evaluate = movementSettings.DuckShake.Evaluate(_movementTime);
            var newRotation = new Vector3(evaluate, evaluate, evaluate);
            item.TransformUtilities.SetScale(newRotation);
        }

        private void CompleteMovement(IMovable item)
        {
            item.TransformUtilities.SetScale(item.TransformUtilities.InitScale);
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