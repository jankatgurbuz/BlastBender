using System.Collections.Generic;
using Blast.View;
using BoardItems;
using Cysharp.Threading.Tasks;
using Global.Controller;
using Util.Movement.States;
using Zenject;

namespace Blast.Controller
{
    public class MovementController : IStartable, ITickable
    {
        private readonly MovementSettings _movementSettings;
        private readonly IGridController _gridController;

        private HashSet<IMovable> _list;
        private List<IMovable> _removeList;

        public MovementController(MovementSettings movementSettings, IGridController gridController)
        {
            _movementSettings = movementSettings;
            _gridController = gridController;
        }

        public async UniTask Start()
        {
            _list = new HashSet<IMovable>();
            _removeList = new List<IMovable>();
            await UniTask.CompletedTask;
        }

        public void Register(IMovable item, IMoveState initState)
        {
            var movementStrategy = item.MovementStrategy;

            if (_list.Add(item))
            {
                movementStrategy.Current = initState;
                movementStrategy.ResetAllStates();
            }
            else
            {
                switch (movementStrategy.Current)
                {
                    case FinishState:
                        movementStrategy.ResetAllStates();
                        movementStrategy.Current = movementStrategy.StartMovement;
                        break;
                    case StartState startState:
                        startState.SetTargetPosition(item.Row, item.Column, _gridController);
                        break;
                    case ShakeState:
                        movementStrategy.ResetAllStates();
                        movementStrategy.Current = initState;
                        break;
                }
            }
        }

        public void Tick()
        {
            if (_list == null)
                return;

            foreach (var item in _list)
            {
                item.MovementStrategy.Current =
                    item.MovementStrategy.Current.DoState(
                        item.MovementStrategy, item, _movementSettings, _gridController);

                if (item.MovementStrategy.Current.AllMovementsComplete)
                {
                    _removeList.Add(item);
                }
            }

            foreach (var item in _removeList)
            {
                _list.Remove(item);
            }

            foreach (var item in _removeList)
            {
                item.MovementStrategy.AllMovementComplete?.Invoke(item);
            }

            _removeList.Clear();
        }

        public void RemoveIfInFinishState(IMovable boardItem)
        {
            if (boardItem.MovementStrategy.Current is FinishState)
            {
                if (_list.Contains(boardItem))
                {
                    _list.Remove(boardItem);
                }
            }
        }
    }
}