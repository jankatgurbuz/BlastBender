using System.Collections;
using System.Collections.Generic;
using Blast.Controller;
using Blast.View;
using Cysharp.Threading.Tasks;
using LevelGenerator.GridSystem.View;
using UnityEngine;
using Zenject;

namespace LevelGenerator.Controller
{
    public class LGGridController : BaseGridController
    {
        public LGGridController(DiContainer container, IGridView gridView) : base(gridView)
        {
        }
    }
}