using Blast.Controller;
using Blast.View;
using BoardItems;
using UnityEngine;
using Util.Movement.Strategies;
using Util.Pool.BeadEffect;

namespace Util.Movement.States
{
    public class CombineState : IMoveState
    {
        private const float MoveTime = 0.15f;
        private const float Offset = 0.3f;

        private bool _isSetupComplete;
        public bool AllMovementsComplete { get; set; }

        private int _rowOffset;
        private int _columnOffset;
        private float _elapsedTime;
        private Vector3 _firstMovePosition;
        private Vector3 _secondMovePosition;
        private Vector3 _startPosition;

        private CombineMovementState _combineMovementState = CombineMovementState.First;
        private RectangleBeadCombinationEffectView _rectangleBeadCombinationEffect;


        private enum CombineMovementState
        {
            First,
            Second
        }

        private void Initialize(IMovable item)
        {
            _startPosition = item.TransformUtilities.GetPosition();

            var x = _startPosition.x - _columnOffset * Offset;
            var y = _startPosition.y - _rowOffset * Offset;
            _firstMovePosition = new Vector3(x, y, _startPosition.z);

            x = _startPosition.x + _columnOffset;
            y = _startPosition.y + _rowOffset;
            _secondMovePosition = new Vector3(x, y, _startPosition.z);

            _rectangleBeadCombinationEffect = RectangleBeadCombinationEffectPool.Instance.Retrieve();
        }

        public void SetParam(int rowOffset, int columnOffset)
        {
            _rowOffset = rowOffset;
            _columnOffset = columnOffset;
        }

        public IMoveState DoState(IMovementStrategy movementStrategy, IMovable item,
            MovementSettings movementSettings,
            IGridController gridController)
        {
            if (!_isSetupComplete)
            {
                Initialize(item);
                _isSetupComplete = true;
            }

            switch (_combineMovementState)
            {
                case CombineMovementState.First:
                    FirstState(item);
                    break;
                case CombineMovementState.Second:
                    SecondState(item);
                    break;
            }

            return this;
        }

        private void FirstState(IMovable item)
        {
            if (_elapsedTime < MoveTime)
            {
                item.TransformUtilities.SetPosition(
                    Vector3.Lerp(_startPosition, _firstMovePosition, _elapsedTime / MoveTime));

                _rectangleBeadCombinationEffect.SetPosition(item.TransformUtilities.GetPosition());

                _elapsedTime += Time.deltaTime;
                return;
            }

            item.TransformUtilities.SetPosition(_firstMovePosition);
            _elapsedTime = 0;
            _combineMovementState = CombineMovementState.Second;
        }

        private void SecondState(IMovable item)
        {
            if (_elapsedTime < MoveTime)
            {
                item.TransformUtilities.SetPosition(
                    Vector3.Lerp(_startPosition, _secondMovePosition, _elapsedTime / MoveTime));

                _rectangleBeadCombinationEffect.SetPosition(item.TransformUtilities.GetPosition());

                _elapsedTime += Time.deltaTime;
                return;
            }

            item.TransformUtilities.SetPosition(_secondMovePosition);
            AllMovementsComplete = true;
            RectangleBeadCombinationEffectPool.Instance.Return(_rectangleBeadCombinationEffect);
        }

        public void ResetState()
        {
            _isSetupComplete = false;
            AllMovementsComplete = false;
            _elapsedTime = 0;
            _combineMovementState = CombineMovementState.First;
        }
    }
}