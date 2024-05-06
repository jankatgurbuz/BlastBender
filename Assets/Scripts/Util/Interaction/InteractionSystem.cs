using System;
using UnityEngine;
using Util.SingletonSystem;

namespace Util.Interaction
{
    public class InteractionSystem : Singleton<InteractionSystem>
    {
        private InteractionPhase _interactionPhase = InteractionPhase.None;
        private event Action<InteractionPhase, Vector2> InteractionSignal;
        public void Update()
        {
            _interactionPhase = InteractionPhase.None;

            Pc();
            Phone();

            //if (_interactionPhase != InteractionPhase.None)
            //{
                InteractionSignal?.Invoke(_interactionPhase, Input.mousePosition);
            //}
        }

        public void Receiver(Action<InteractionPhase, Vector2> action)
        {
            InteractionSignal += action;
        }

        private void Pc()
        {
#if UNITY_EDITOR
            if (Input.GetMouseButtonDown(0))
            {
                _interactionPhase = InteractionPhase.Down;
            }
            else if (Input.GetMouseButton(0))
            {
                _interactionPhase = InteractionPhase.ContinuousPress;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                _interactionPhase = InteractionPhase.Up;
            }
#endif
        }
        private void Phone()
        {
            if (Input.touchCount <= 0)
                return;

            var t = Input.GetTouch(0);

            switch (t.phase)
            {
                case TouchPhase.Began:
                    _interactionPhase = InteractionPhase.Down;
                    break;
                case TouchPhase.Ended:
                    _interactionPhase = InteractionPhase.Up;
                    break;
            }
        }
    }
    public enum InteractionPhase
    {
        None,
        Down,
        Up,
        ContinuousPress
    }
}
