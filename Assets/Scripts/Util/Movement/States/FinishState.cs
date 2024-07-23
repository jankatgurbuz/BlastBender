using Blast.Controller;
using Blast.View;
using BoardItems;
using UnityEngine;
using Util.Movement.Strategies;

namespace Util.Movement.States
{
    public class FinishState : IMoveState
    {
        private Vector3 _initialScale;
        private Vector3 _currentScale;
        private Vector3 _initialPosition;
        private Vector3 _firstMovePosition;
        private Vector3 _secondMovePosition;
        private Vector3 _thirdMovePosition;
        private Vector3 _firstScale;
        private Vector3 _secondScale;

        private float _elapsedTime;
        private int _step; // todo bad check
        private AnimationCurve _curve;

        private bool _isSetupComplete;
        public bool AllMovementsComplete { get; set; }

        public IMoveState DoState(IMovementStrategy movementStrategy, IBoardItem item,
            MovementSettings movementSettings, IGridController gridController)
        {
            if (!_isSetupComplete)
            {
                AssignVariables(item.MovementVisitor.MoveableItem.GetTransform(), item);
                _isSetupComplete = true;
            }

            var t = item.MovementVisitor.MoveableItem.GetTransform();
            switch (_step)
            {
                case 0:
                    Movement(t, _firstScale, _firstMovePosition, 0);
                    break;
                case 1:
                    Movement(t, _secondScale, _secondMovePosition, 0.5f);
                    break;
                case 2:
                    Movement(t, _secondScale, _thirdMovePosition, 0.5f);
                    break;
            }

            return movementStrategy.FinishMovement;
        }

        public void ResetState()
        {
            _isSetupComplete = false;
            AllMovementsComplete = false;
            _elapsedTime = 0;
            _step = 0;
        }

        public void Movement(Transform transform, Vector3 targetScale, Vector3 targetMove, float duration)
        {
            if (_elapsedTime < duration)
            {
                var t = Mathf.Clamp01(_elapsedTime / duration);
                var curveValue = _curve.Evaluate(t);

                transform.localPosition = _initialPosition + (targetMove - _initialPosition) * curveValue;
                transform.localScale = Vector3.Lerp(transform.localScale, targetScale, t);

                _elapsedTime += Time.deltaTime;
                // Debug.Log(_elapsedTime + "-" + duration);
                return;
            }

            // transform.localPosition = targetMove;
            // transform.localScale = targetScale;
            _elapsedTime = 0;
            _step++;
            Debug.Log("test" + _step);
            if (_step == 3)
            {
                AllMovementsComplete = true;
            }
        }

        private void AssignVariables(Transform transform, IBoardItem item)
        {
            _initialScale = transform.localScale;
            _initialPosition = transform.localPosition;

            _firstScale = new Vector3(_initialScale.x + 0.05f, _initialScale.y - 0.025f, _initialScale.z);
            _secondScale = new Vector3(_initialScale.x + 0.025f, _initialScale.y, _initialScale.z);

            _firstMovePosition = _initialPosition + new Vector3(0, -0, 0);
            _secondMovePosition = _initialPosition + new Vector3(0, 0.1f, 0);
            _thirdMovePosition = _initialPosition + new Vector3(0, -0.1f, 0);

            _curve = AnimationCurve.Linear(0, 0, 1, 1);
        }
    }
}