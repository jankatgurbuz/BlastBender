using Blast.Controller;
using Blast.View;
using BoardItems;
using UnityEngine;
using Util.Movement.Strategies;

namespace Util.Movement.States
{
    public class FinishState : IMoveState
    {
        private const float ScaleRate = 0.025f;
        private const float ScaleRateTop = 0.035f;
        private const float Duration = 0.1f;

        private Vector3 _initialScale;
        private Vector3 _currentScale;
        private Vector3 _initialPosition;
        private Vector3 _firstMovePosition;
        private Vector3 _secondMovePosition;
        private Vector3 _targetAnimY;
        private Vector3 _targetAnimTopY;

        private float _elapsedTime;
        private int _step;
        private AnimationCurve _curve;
        
        public bool Initialize { get; set; }
        public bool AllMovementsComplete { get; set; }

        public void Test(Transform transform, Vector3 targetScale, Vector3 targetMove)
        {
            if (_elapsedTime < Duration)
            {
                var t = Mathf.Clamp01(_elapsedTime / Duration);
                var curveValue = _curve.Evaluate(t);

                transform.localPosition = _initialPosition + (targetMove - _initialPosition) * curveValue;
                transform.localScale = Vector3.Lerp(transform.localScale, targetScale, t);

                _elapsedTime += Time.deltaTime;
                return;
            }

            transform.localPosition = targetMove;
            transform.localScale = targetScale;
            _elapsedTime = 0;
            _step++;
        }


        public IMoveState DoState(IMovementStrategy movementStrategy, IBoardItem item,
            MovementSettings movementSettings,
            IGridController gridController)
        {
            if (!Initialize)
            {
                Init(item.MovementVisitor.MoveableItem.GetTransform());
                Initialize = true;
            }

            var t = item.MovementVisitor.MoveableItem.GetTransform();
            switch (_step)
            {
                case 0:
                    Test(t, _targetAnimY, _firstMovePosition);
                    break;
                case 1:
                    Test(t, _targetAnimTopY, _secondMovePosition);
                    break;
                case 2:
                    Test(t, _initialScale, _initialPosition);
                    break;
                case 3:
                    AllMovementsComplete = true;
                    break;
            }

            return movementStrategy.FinishMovement;
        }

        private void Init(Transform transform)
        {
            _initialScale = transform.localScale;
            _initialPosition = transform.localPosition;

            _targetAnimY = new Vector3(_initialScale.x, _initialScale.y + ScaleRate, _initialScale.z);
            _targetAnimTopY = new Vector3(_initialScale.x, _initialScale.y + ScaleRateTop, _initialScale.z);

            _firstMovePosition = _initialPosition + new Vector3(0, -ScaleRate, 0);
            _secondMovePosition = _initialPosition + new Vector3(0, ScaleRateTop, 0);

            _curve = AnimationCurve.Linear(0, 0, 1, 1);
        }

        public void Restart(bool withoutInitialize)
        {
            if (!withoutInitialize)
            {
               
            }
            Initialize = false;
            AllMovementsComplete = false;
            _elapsedTime = 0;
            _step = 0;
        }
    }
}