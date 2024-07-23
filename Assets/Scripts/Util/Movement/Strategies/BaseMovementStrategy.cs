using System.Threading;
using Blast.Controller;
using Blast.View;
using BoardItems;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Util.Movement.States;
using Util.Movement.Strategies;


namespace Util.Handlers.Strategies
{
    public class BaseMovementStrategy : IMovementStrategy
    {
        private CancellationTokenSource _cancellationTokenSource = new();
        public bool IsPlayShake { get; set; }
        public bool IsPlayStartMovement { get; set; }
        public bool IsPlayFinalMovement { get; set; }

        private bool _kill;

        public bool Kill
        {
            get => _kill;
            set
            {
                if (!value) return;

                _kill = false;
                _cancellationTokenSource?.Cancel();
                _cancellationTokenSource?.Dispose();
                _cancellationTokenSource = new CancellationTokenSource();
            }
        }

        public async UniTask Shake2(Transform transform)
        {
            var shakeAnglePositive = Quaternion.Euler(new Vector3(0, 0, 9));
            var shakeAngleNegative = Quaternion.Euler(new Vector3(0, 0, -9));
            const float duration = 0.075f;
            var token = _cancellationTokenSource.Token;

            IsPlayShake = true;

            await AnimateRotation(transform, shakeAnglePositive, duration, token);
            await AnimateRotation(transform, shakeAngleNegative, duration, token);
            await AnimateRotation(transform, shakeAnglePositive, duration, token);
            await AnimateRotation(transform, Quaternion.identity, duration, token);
            IsPlayShake = false;
        }

        public async UniTask StartMovement2(Transform transform)
        {
            const float scaleRate = 0.01f;
            const float duration = 0.2f;
            var token = _cancellationTokenSource.Token;

            IsPlayStartMovement = true;

            var temp = transform.localScale;
            var targetAnimX = new Vector3(temp.x - scaleRate, temp.y, temp.z);
            var targetAnimY = new Vector3(targetAnimX.x, temp.y + scaleRate, temp.z);

            await UniTask.WhenAll(
                AnimateScale(transform, targetAnimX, duration, token),
                AnimateScale(transform, targetAnimY, duration, token));

            IsPlayStartMovement = false;
        }

        public async UniTask FinalMovement2(Transform transform, Vector3 currentScale)
        {
            const float scaleRate = 0.025f;
            const float scaleRateTop = 0.035f;
            const float duration = 0.1f;
            var token = _cancellationTokenSource.Token;

            var temp = transform.localScale;
            var targetAnimY = new Vector3(currentScale.x, temp.y + scaleRate, temp.z);
            var targetAnimTopY = new Vector3(currentScale.x, temp.y + scaleRateTop, temp.z);

            var tempPosition = transform.localPosition;
            var firstMove = transform.localPosition + new Vector3(0, -scaleRate, 0);
            var secondMove = transform.localPosition + new Vector3(0, scaleRateTop, 0);

            IsPlayFinalMovement = true;

            // todo add movement settings
            var curve = AnimationCurve.Linear(0, 0, 1, 1);

            await UniTask.WhenAll(
                AnimateScale(transform, targetAnimY, duration, token),
                AnimateMove(transform, firstMove, duration, curve, token)
            );

            await UniTask.WhenAll(
                AnimateScale(transform, targetAnimTopY, duration, token),
                AnimateMove(transform, secondMove, duration, curve, token)
            );

            await AnimateMove(transform, tempPosition, duration, curve, token);
            transform.localScale = currentScale;
            IsPlayFinalMovement = false;
        }


        private async UniTask AnimateScale(Transform transform, Vector3 targetScale, float duration,
            CancellationToken token)
        {
            var elapsed = 0f;
            while (elapsed < duration && !token.IsCancellationRequested)
            {
                var t = elapsed / duration;
                transform.localScale = Vector3.Lerp(transform.localScale, targetScale, t);
                elapsed += Time.deltaTime;
                await UniTask.Yield(PlayerLoopTiming.Update);
            }

            transform.localScale = targetScale;
        }

        private async UniTask AnimateRotation(Transform transform, Quaternion targetRotation, float duration,
            CancellationToken token)
        {
            var elapsed = 0f;
            while (elapsed < duration && !token.IsCancellationRequested)
            {
                transform.localRotation =
                    Quaternion.Lerp(transform.localRotation, targetRotation, duration);
                elapsed += Time.deltaTime;
                await UniTask.Yield(PlayerLoopTiming.Update);
            }

            transform.localRotation = targetRotation;
        }

        public async UniTask AnimateMove(Transform transform, Vector3 target, float duration, AnimationCurve curve,
            CancellationToken token)
        {
            var startPosition = transform.localPosition;
            var elapsed = 0f;

            while (elapsed < duration && !token.IsCancellationRequested)
            {
                elapsed += Time.deltaTime * 0.75f;
                var t = Mathf.Clamp01(elapsed / duration);
                var curveValue = curve.Evaluate(t);
                transform.localPosition = startPosition + (target - startPosition) * curveValue;
                await UniTask.Yield(PlayerLoopTiming.Update);
            }

            transform.localPosition = target;
        }


        public IMoveState StartMovement { get; set; } = new StartState();
        public IMoveState FinishMovement { get; set; } = new FinishStateTest();
        public IMoveState Current { get; set; }

        public void Restart()
        {
            StartMovement.ResetState();
            FinishMovement.ResetState();
        }
    }
}


