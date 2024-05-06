using System.Collections;
using System.Collections.Generic;
using Blast.View;
using UnityEngine;
using Zenject;

namespace Blast.Controller
{
    public class GridController : BaseGridController
    {
        public GridController(IGridView gridView) : base(gridView)
        {
        }
    }
}