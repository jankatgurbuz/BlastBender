using System.Collections;
using System.Collections.Generic;
using Blast.Controller;
using UnityEngine;
using Util.Pool;
using System;

namespace LevelGenerator.Controller
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