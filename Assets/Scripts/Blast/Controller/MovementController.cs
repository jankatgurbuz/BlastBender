using System;
using System.Collections.Generic;
using Blast.View;
using BoardItems;
using Cysharp.Threading.Tasks;
using Global.Controller;
using UnityEngine;
using Zenject;

namespace Blast.Controller
{
    public class MovementController : IStartable, ITickable
    {
        private readonly MovementSettings _movementSettings;
        private readonly IGridController _gridController;
        private Dictionary<ValueTuple<int, int>, MovementData> _movementItems;
        private ValueTuple<int, int>[] _keys;

        public MovementController(MovementSettings movementSettings, IGridController gridController)
        {
            _movementSettings = movementSettings;
            _gridController = gridController;
        }

        public async UniTask Start()
        {
            _movementItems = new Dictionary<(int, int), MovementData>();
            await UniTask.CompletedTask;
        }

        public void Register(IBoardItem item, int row, int column)
        {
            var firstPosition = item.GetPosition();
            if (_movementItems.ContainsKey((row, column)))
            {
                firstPosition = _movementItems[(row, column)].FirstPosition;
                _movementItems.Remove((row, column));
            }
            else
            {
                item.MovementVisitor.StartMovement();
            }

            _movementItems.Add((item.Row, item.Column), new MovementData()
            {
                BoardItem = item,
                FirstPosition = firstPosition,
                TargetPosition = _gridController.CellToLocal(item.Row, item.Column)
            });
        }

        public void Tick()
        {
            if (_movementItems == null)
                return;

            var deltaTime = Time.deltaTime;
            var removeList = new List<IBoardItem>();
            foreach (var item in _movementItems.Values)
            {
                if (!item.BoardItem.MovementVisitor.IsMoveable)
                    continue;

                item.BoardItem.MovementVisitor.MovementTime += deltaTime * 0.2f;
                var y = item.FirstPosition.y -
                        _movementSettings.AnimationCurve.Evaluate(item.BoardItem.MovementVisitor.MovementTime);
                y = Mathf.Clamp(y, item.TargetPosition.y, 1000); // todo: magic number !!! 
                var newPosition = new Vector3(item.FirstPosition.x, y, item.FirstPosition.z);
                item.BoardItem.SetPosition(newPosition);

                if (Mathf.Approximately(y, item.TargetPosition.y))
                {
                    item.BoardItem.IsMove = false;
                    item.BoardItem.MovementVisitor.FinalizeMovementWithBounce();
                    item.BoardItem.MovementVisitor.MovementTime = 0;
                    removeList.Add(item.BoardItem);
                }
            }

            foreach (var removeItem in removeList)
            {
                _movementItems.Remove((removeItem.Row, removeItem.Column));
            }
        }

        private class MovementData
        {
            public IBoardItem BoardItem;
            public Vector3 FirstPosition;
            public Vector3 TargetPosition;
        }
    }
}