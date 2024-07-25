using Blast.Controller;
using Blast.View;
using BoardItems;
using UnityEngine;
using Util.Movement.Strategies;

namespace Util.Movement.States
{
    public class StartState : IMoveState
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

        public IMoveState DoState(IMovementStrategy movementStrategy, IBoardItem item,
            MovementSettings movementSettings, IGridController gridController)
        {
            if (!_isSetupComplete)
            {
                AssignVariables(item, gridController);
                _isSetupComplete = true;
                item.IsMove = true;
            }

            Scale(item);
            return Movement(movementStrategy, item, movementSettings);
        }

        private void AssignVariables(IBoardItem item, IGridController gridController)
        {
            InitializeScale(item);
            InitializePosition(item, gridController);
        }

        private void InitializeScale(IBoardItem item)
        {
            if (item.IsMove) return;

            item.TransformUtilities.SetRotation(Vector3.zero);
            var temp = item.TransformUtilities.GetScale();
            _scaleTarget = new Vector3(temp.x - ScaleRate, temp.y + ScaleRate, temp.z);
        }

        private void InitializePosition(IBoardItem item, IGridController gridController)
        {
            if (item.IsMove)
            {
                _targetPosition = gridController.CellToLocal(item.Row, item.Column);
                return;
            }

            _targetPosition = gridController.CellToLocal(item.Row, item.Column);
            _firstPosition = item.TransformUtilities.GetPosition();
        }

        private IMoveState Movement(IMovementStrategy movementStrategy, IBoardItem boardItem,
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
                boardItem.IsMove = false;
                return movementStrategy.FinishMovement;
            }

            return movementStrategy.StartMovement;
        }

        private void Scale(IBoardItem item)
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

        public void SetTargetPosition(IBoardItem item, IGridController gridController)
        {
            _targetPosition = gridController.CellToLocal(item.Row, item.Column);
        }

        public void ResetState()
        {
            _scaleTimeElapsed = 0f;
            _movementTime = 0;
            _isSetupComplete = false;
        }
    }
}