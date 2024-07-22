using Blast.Controller;
using Blast.View;
using BoardItems;
using UnityEngine;
using Util.Movement.Strategies;

namespace Util.Movement.States
{
    public class StartState : IMoveState
    {
        private const float ScaleRate = 0.11f;
        private const float Duration = 0.2f;

        private float _scaleTimeElapsed;
        private float _movementTime;

        private Vector3 _firstPosition;
        private Vector3 _scaleTarget;
        private Vector3 _targetPosition;
        public bool Initialize { get; set; }
        public bool AllMovementsComplete { get; set; }

        private void Init(IBoardItem item, IGridController gridController)
        {
            var temp = item.MovementVisitor.MoveableItem.GetTransform().localScale;
            _firstPosition = item.GetPosition();
            _targetPosition = gridController.CellToLocal(item.Row, item.Column);
            _scaleTarget = new Vector3(temp.x - ScaleRate, temp.y + ScaleRate, temp.z);
        }

        public IMoveState DoState(IMovementStrategy movementStrategy, IBoardItem item,
            MovementSettings movementSettings, IGridController gridController)
        {
            if (!Initialize)
            {
                Init(item, gridController);
                Initialize = true;
            }

            Scale(item.MovementVisitor.MoveableItem.GetTransform());
            return Movement(movementStrategy, item, movementSettings);
        }
        private IMoveState Movement(IMovementStrategy movementStrategy, IBoardItem boardItem,
            MovementSettings movementSettings)
        {
            _movementTime += Time.deltaTime * 0.2f;
            var y = _firstPosition.y -
                    movementSettings.AnimationCurve.Evaluate(_movementTime);
            y = Mathf.Clamp(y, _targetPosition.y, 1000);
            var newPosition = new Vector3(_firstPosition.x, y, _firstPosition.z);
            boardItem.SetPosition(newPosition);

            if (Mathf.Approximately(y, _targetPosition.y))
            {
                boardItem.IsMove = false;
                AllMovementsComplete = true;
                return movementStrategy.StartMovement;
            }

            return movementStrategy.StartMovement;
        }

        private void Scale(Transform transorm)
        {
            if (_scaleTimeElapsed < Duration)
            {
                var t = _scaleTimeElapsed / Duration;
                transorm.localScale = Vector3.Lerp(transorm.localScale, _scaleTarget, t);
                _scaleTimeElapsed += Time.deltaTime;
                return;
            }

            transorm.localScale = _scaleTarget;
        }
        public void Restart(bool withoutInitialize)
        {
            if (!withoutInitialize)
            {
                
            }
            AllMovementsComplete = false;
            Initialize = false;
            _scaleTimeElapsed = 0f;
            _movementTime = 0;
        }
    }
}