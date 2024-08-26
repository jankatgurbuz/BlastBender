using System.Collections.Generic;
using Blast.View;
using BoardItems;
using Cysharp.Threading.Tasks;
using Gameplay.Movement.States.BaseState;
using Global.Controller;
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
                if (movementStrategy.Current.IsFirstMovement)
                {
                    var position = _gridController.CellToLocal(item.Row, item.Column);
                    ((IPositionSetter)movementStrategy.Current).SetTargetPosition(position);
                }
                else
                {
                    movementStrategy.ResetAllStates();
                    movementStrategy.Current = initState;
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
            if (boardItem.MovementStrategy.Current == null) return;

            if (boardItem.MovementStrategy.Current.IsLastMovement)
            {
                if (_list.Contains(boardItem))
                {
                    _list.Remove(boardItem);
                }
            }
        }
    }
}