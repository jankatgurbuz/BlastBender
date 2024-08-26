using Blast.Controller;
using Blast.View;
using BoardItems;
using Gameplay.Movement.Strategies;
using UnityEngine;

namespace Gameplay.Movement.States.BaseState
{
    public class StartState : IMoveState,IPositionSetter
    {
        private const float ScaleRate = 0.025f;
        private const float Duration = 0.5f;
        private const float MovementOffset = 0.35f;

        private float _scaleTimeElapsed;
        private float _movementTime;

        private Vector3 _firstPosition;
        private Vector3 _scaleTarget;
        private Vector3 _targetPosition;

        private bool _isSetupComplete;
        public bool AllMovementsComplete { get; set; }
        public bool IsLastMovement { get; set; }
        public bool IsFirstMovement { get; set; }

        public IMoveState DoState(IMovementStrategy movementStrategy, IMovable item,
            MovementSettings movementSettings, IGridController gridController)
        {
            if (!_isSetupComplete)
            {
                AssignVariables(item, gridController);
                _isSetupComplete = true;
                item.IsMoving = true;
                IsFirstMovement = true;
            }

            Scale(item);
            return Movement(movementStrategy, item, movementSettings);
        }

        private void AssignVariables(IMovable item, IGridController gridController)
        {
            InitializeScale(item);
            InitializePosition(item, gridController);
        }

        private void InitializeScale(IMovable item)
        {
            if (item.IsMoving) return;

            item.TransformUtilities.SetRotation(Vector3.zero);
            var temp = item.TransformUtilities.GetScale();
            _scaleTarget = new Vector3(temp.x - ScaleRate, temp.y + ScaleRate, temp.z);
        }

        private void InitializePosition(IMovable item, IGridController gridController)
        {
            if (item.IsMoving)
            {
                _targetPosition = gridController.CellToLocal(item.Row, item.Column);
                return;
            }

            _targetPosition = gridController.CellToLocal(item.Row, item.Column);
            _firstPosition = item.TransformUtilities.GetPosition();
        }

        private IMoveState Movement(IMovementStrategy movementStrategy, IMovable boardItem,
            MovementSettings movementSettings)
        {
            _movementTime += Time.deltaTime * MovementOffset;
            var evaluate = movementSettings.AnimationCurve.Evaluate(_movementTime);
            var clampedY = Mathf.Clamp(_firstPosition.y - evaluate, _targetPosition.y, 1000);
            var newPosition = new Vector3(_firstPosition.x, clampedY, _firstPosition.z);
            boardItem.TransformUtilities.SetPosition(newPosition);

            if (Mathf.Approximately(clampedY, _targetPosition.y))
            {
                boardItem.TransformUtilities.SetPosition(_targetPosition);
                boardItem.IsMoving = false;
                return movementStrategy.FinishMovement;
            }

            return movementStrategy.StartMovement;
        }

        private void Scale(IMovable item)
        {
            if (_scaleTimeElapsed < Duration)
            {
                var t = _scaleTimeElapsed / Duration;
                item.TransformUtilities.SetScale(Vector3.Lerp(item.TransformUtilities.GetScale(), _scaleTarget, t));
                _scaleTimeElapsed += Time.deltaTime;
                return;
            }

            item.TransformUtilities.SetScale(_scaleTarget);
        }

        public void SetTargetPosition(Vector3 position)
        {
            _targetPosition = position;
        }

        public void ResetState()
        {
            _scaleTimeElapsed = 0f;
            _movementTime = 0;
            _isSetupComplete = false;
        }
    }
}