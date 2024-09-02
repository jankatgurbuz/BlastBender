using System;
using Blast.Controller;
using UnityEngine;

namespace LevelGenerator.Scripts.Controller
{
    public interface ILGGridInteractionController : ILGStart
    {
        public event Action<int, int> Down;
        public event Action<int, int> Up;
        public event Action<int, int> NoneTouch;
    }

    public class LGGridInteractionController : BaseGridInteractionController, ILGGridInteractionController
    {
        public LGGridInteractionController(Camera camera, bool isContinuousPress, LGGridController gridController) :
            base(camera, isContinuousPress, gridController)
        {
        }

        public void Start()
        {
            Receiver();
        }
    }
}