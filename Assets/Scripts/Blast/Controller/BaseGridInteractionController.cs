using System;
using LevelGenerator.Controller;
using UnityEngine;
using Util.Interaction;
using Util.Pool;

namespace Blast.Controller
{
    public interface IGridInteractionController
    {
        public void Receiver();
        public Camera Camera { get; }
        public bool IsContinuousPress { get; }

        public event Action<int, int> Down;
        public event Action<int, int> Up;
        public event Action<int, int> NoneTouch;
    }
    public abstract class BaseGridInteractionController : IGridInteractionController
    {
        public Camera Camera { get; }
        public bool IsContinuousPress { get; }
        private readonly IGridController _gridController;
        public event Action<int, int> Down;
        public event Action<int, int> Up;
        public event Action<int, int> NoneTouch;

        public BaseGridInteractionController(Camera camera, bool isContinuousPress, IGridController gridController)
        {
            Camera = camera;
            IsContinuousPress = isContinuousPress;
            _gridController = gridController;
        }

        public void Receiver()
        {
            InteractionSystem.Instance.Receiver(OnClick);
        }

        public virtual void OnClick(InteractionPhase phase, Vector2 vector)
        {
            Action(phase, vector);
        }

        protected void Action(InteractionPhase phase, Vector3 vec)
        {
            if (IsContinuousPress && phase == InteractionPhase.None)
            {
                var (row, column) = GetDrawPosition(vec);
                NoneTouch?.Invoke(column, row);
            }
            else if (phase == InteractionPhase.Down)
            {
                var (row, column) = GetDrawPosition(vec);
                Down?.Invoke(column, row);
            }
            else if (phase == InteractionPhase.Up)
            {
                var (row, column) = GetDrawPosition(vec);
                Up?.Invoke(column, row);
            }
        }

        protected (int, int) GetDrawPosition(Vector3 vec)
        {
            var cellSize = _gridController.GetCellSize();
            var mousePos = Camera.ScreenToWorldPoint(vec);
            int row = Mathf.RoundToInt(mousePos.x / cellSize.x);
            int column = Mathf.RoundToInt(mousePos.y / cellSize.y);

            return (row, column);
        }

        
    }
}