using UnityEngine;

namespace Blast.View
{
    [CreateAssetMenu(fileName = "MovementSettings", menuName = "MovementSettings")]
    public class MovementSettings : ScriptableObject
    {
        public AnimationCurve AnimationCurve;
        public AnimationCurve FinalMovementAnimationCurve;
    }
}
