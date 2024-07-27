using UnityEngine;

namespace Blast.View
{
    [CreateAssetMenu(fileName = "MovementSettings", menuName = "BlastBender/Game/MovementSettings")]
    public class MovementSettings : ScriptableObject
    {
        public AnimationCurve AnimationCurve;
        public AnimationCurve FinalMovementAnimationCurve;
        public AnimationCurve Shake;
    }
}
