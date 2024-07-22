using System;
using System.Collections.Generic;
using Blast.View;
using BoardItems;
using BoardItems.Bead;
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
        private Dictionary<ValueTuple<int, int>, IBoardItem> _movementItems;
        private List<IBoardItem> _removeList;
        private ValueTuple<int, int>[] _keys;

        public MovementController(MovementSettings movementSettings, IGridController gridController)
        {
            _movementSettings = movementSettings;
            _gridController = gridController;
        }

        public async UniTask Start()
        {
            _movementItems = new Dictionary<(int, int), IBoardItem>();
            _removeList = new List<IBoardItem>();
            await UniTask.CompletedTask;
        }

        public void Register(IBoardItem item, int number)
        {
            if (!item.MovementVisitor.IsMovementSupported) return;
            // var firstPosition = item.GetPosition();
            // if (_movementItems.ContainsKey((row, column)))
            // {
            //     firstPosition = _movementItems[(row, column)].FirstPosition;
            //     _movementItems.Remove((row, column));
            // }
            // else
            // {
            //     item.MovementVisitor.StartMovement();
            // }

            // _movementItems.Add((item.Row, item.Column), new MovementData()
            // {
            //     BoardItem = item,
            //     FirstPosition = firstPosition,
            //     TargetPosition = _gridController.CellToLocal(item.Row, item.Column)
            // });


            if (_movementItems.ContainsKey((number, item.Column)))
            {
                if (number == 100)
                {
                    Debug.Log("Girdi");
                }

                item.MovementVisitor.MovementStrategy.Restart();
                item.MovementVisitor.MovementStrategy.Current = item.MovementVisitor.MovementStrategy.StartMovement;
            }
            else
            {
                Debug.Log(item.Row + "----" + item.Column);
                item.MovementVisitor.MovementStrategy.Restart();
                item.MovementVisitor.MovementStrategy.Current = item.MovementVisitor.MovementStrategy.StartMovement;
                _movementItems.Add((item.Row, item.Column), item);
            }
        }

        public void Tick()
        {
            if (_movementItems == null)
                return;

            foreach (var item in _movementItems.Values)
            {
                // if (!item.MovementVisitor.IsMoveable)
                //     continue;

                // //todo Movement!
                // // ilerde bunu kaldirmayi calis register da kontrol etitn zaten
                //
                // if (!item.MovementVisitor.IsMovementSupported) continue;

                item.MovementVisitor.MovementStrategy.Current =
                    item.MovementVisitor.MovementStrategy.Current.DoState(
                        item.MovementVisitor.MovementStrategy, item, _movementSettings, _gridController);

                if (item.MovementVisitor.MovementStrategy.Current.AllMovementsComplete)
                {
                    _removeList.Add(item);
                    item.IsMove = false;
                }

                // item.BoardItem.MovementVisitor.MovementTime += deltaTime * 0.2f;
                // var y = item.FirstPosition.y -
                //         _movementSettings.AnimationCurve.Evaluate(item.BoardItem.MovementVisitor.MovementTime);
                // y = Mathf.Clamp(y, item.TargetPosition.y, 1000); // todo: magic number !!! 
                // var newPosition = new Vector3(item.FirstPosition.x, y, item.FirstPosition.z);
                // item.BoardItem.SetPosition(newPosition);
                //
                // if (Mathf.Approximately(y, item.TargetPosition.y))
                // {
                //     item.BoardItem.IsMove = false;
                //     item.BoardItem.MovementVisitor.FinalizeMovementWithBounce();
                //     item.BoardItem.MovementVisitor.MovementTime = 0;
                //     removeList.Add(item.BoardItem);
                // }
            }

            foreach (var removeItem in _removeList)
            {
                var check=_movementItems.Remove((removeItem.Row,removeItem.Column));
                if (!check)
                {
                    Debug.Log("silemedim "+removeItem.Row+" ------ "+removeItem.Column);

                    foreach (var test in _movementItems)
                    {
                        Debug.Log(test.Key.ToString());
                    }
                }
            }

            _removeList.Clear();
        }

        // private class MovementData
        // {
        //     public IBoardItem BoardItem;
        //     public Vector3 FirstPosition;
        //     public Vector3 TargetPosition;
        // }
    }
}