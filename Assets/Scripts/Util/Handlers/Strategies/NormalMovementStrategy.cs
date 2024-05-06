using DG.Tweening;
using UnityEngine;

namespace Util.Handlers.Strategies
{
    public class NormalMovementStrategy : IMovementStrategy
    {
        public Sequence FinalMovement(Transform transform, Vector3 currentScale)
        {
            const float scaleRate = 0.05f;
            const float scaleRateTop = 0.07f;

            var sequence = CreateSequence();
            sequence.Append(transform.DOScaleY(-scaleRate, 0.1f).SetRelative());
            sequence.Join(transform.DOMoveY(-scaleRate / 2f, 0.1f).SetRelative());
            sequence.Append(transform.DOScaleY(scaleRateTop, 0.1f).SetRelative());
            sequence.Join(transform.DOMoveY(scaleRateTop / 2f, 0.1f).SetRelative());
            sequence.Append(transform.DOScale(currentScale, 0.1f));
            sequence.Join(transform.DOMoveY(-0.02f / 2f, 0.1f).SetRelative());

            return sequence;
        }

        public Sequence StartMovement(Transform transform)
        {
            const float scaleRate = 0.01f;
            var sequence = CreateSequence();
            sequence.Append(transform.DOScaleX(-scaleRate, 0.1f).SetRelative());
            sequence.Join(transform.DOScaleY(scaleRate, 0.1f).SetRelative());
            return sequence;
        }

        public Sequence Shake(Transform transform)
        {
            var shakeRatio = new Vector3(0, 0, 9);
            var shaker = CreateSequence();
            shaker.Append(transform.DORotate(shakeRatio, .075f)).Append(transform.DORotate(-shakeRatio, .075f))
                .Append(transform.DORotate(shakeRatio, .075f)).Append(transform.DORotate(Vector3.zero, .075f));
            return shaker;
        }
        private static Sequence CreateSequence()
        {
            var sequence = DOTween.Sequence();
            sequence.SetAutoKill(false);
            sequence.Pause();
            return sequence;
        }
    }
}