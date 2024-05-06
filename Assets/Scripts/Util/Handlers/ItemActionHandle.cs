using System;
using System.Collections.Generic;
using DG.Tweening;

namespace Util.Handlers
{
    public class ItemActionHandle
    {
        private readonly Dictionary<Actions, ItemAction> _actions = new();

        public void ReplaceAction(Actions key, Sequence sequence, bool overrideAction = false,
            Action onKillAction = null, params Actions[] killDependency)
        {
            if (!_actions.ContainsKey(key))
            {
                _actions.Add(key, CreateItemAction(sequence, onKillAction, killDependency));
            }
            else if (overrideAction)
            {
                _actions[key] = CreateItemAction(sequence, onKillAction, killDependency);
            }
        }

        private static ItemAction CreateItemAction(Sequence sequence, Action onKillAction, Actions[] killDependency)
        {
            return new ItemAction()
            {
                Sequence = sequence,
                OnKillAction = onKillAction,
                KillDependency = killDependency
            };
        }

        public bool IsPlaying(Actions key)
        {
            return _actions.TryGetValue(key, out var action) && action.Sequence.IsPlaying();
        }

        public void RestartSequence(Actions key)
        {
            if (!_actions.TryGetValue(key, out var action)) return;
            action.Sequence.Restart();
            KillDependency(action);
        }

        private void KillDependency(ItemAction action)
        {
            var dependency = action.KillDependency;

            foreach (var item in dependency)
            {
                if (!_actions.TryGetValue(item, out var value)) continue;
                value.Sequence.Pause();
                value.OnKillAction?.Invoke();
            }
        }

        private class ItemAction
        {
            public Sequence Sequence;
            public Action OnKillAction;
            public Actions[] KillDependency;
        }

        public enum Actions
        {
            Shake,
            StartMovement,
            FinalMovement
        }
    }
}