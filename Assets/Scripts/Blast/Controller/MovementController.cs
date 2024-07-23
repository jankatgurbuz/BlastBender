using System;
using System.Collections.Generic;
using Blast.View;
using BoardItems;
using BoardItems.Bead;
using Cysharp.Threading.Tasks;
using Global.Controller;
using UnityEngine;
using Util.Movement.States;
using Zenject;

namespace Blast.Controller
{
    public class MovementController : IStartable, ITickable
    {
        private readonly MovementSettings _movementSettings;
        private readonly IGridController _gridController;

        private HashSet<IBoardItem> _list;
        private List<IBoardItem> _removeList;
        private ValueTuple<int, int>[] _keys;

        public MovementController(MovementSettings movementSettings, IGridController gridController)
        {
            _movementSettings = movementSettings;
            _gridController = gridController;
        }

        public async UniTask Start()
        {
            _list = new HashSet<IBoardItem>();
            _removeList = new List<IBoardItem>();
            await UniTask.CompletedTask;
        }

        public void Register(IBoardItem item)
        {
            var movementStrategy = item.MovementVisitor.MovementStrategy;
    
            if (_list.Add(item))
            {
                movementStrategy.Current = movementStrategy.StartMovement;
                movementStrategy.Restart();
            }
            else
            {
                switch (movementStrategy.Current)
                {
                    case FinishStateTest:
                        movementStrategy.Restart();
                        movementStrategy.Current = movementStrategy.StartMovement;
                        break;
                    case StartState startState:
                        startState.Test(item, _gridController);
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
                item.MovementVisitor.MovementStrategy.Current =
                    item.MovementVisitor.MovementStrategy.Current.DoState(
                        item.MovementVisitor.MovementStrategy, item, _movementSettings, _gridController);

                if (item.MovementVisitor.MovementStrategy.Current.AllMovementsComplete)
                {
                    _removeList.Add(item);
                }
            }

            foreach (var item in _removeList)
            {
                _list.Remove(item);
            }

            _removeList.Clear();
        }

        public void Check(IBoardItem boardItem)
        {
            if (boardItem.MovementVisitor.MovementStrategy.Current is FinishStateTest)
            {
                var check = _list.Contains(boardItem);
                if (check)
                {
                    _list.Remove(boardItem);
                }
            } 
        }
    }
}