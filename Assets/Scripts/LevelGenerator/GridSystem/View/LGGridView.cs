using System.Collections;
using System.Collections.Generic;
using Blast.View;

using NaughtyAttributes;
using UnityEngine;
using Zenject;

namespace LevelGenerator.GridSystem.View
{
    public class LGGridView : MonoBehaviour, IGridView
    {
        [SerializeField] private Grid _grid;
        [SerializeField] private Camera _camera;
        [SerializeField] private GridIndicator _gridIndicatorPrefab;

        public Grid Grid => _grid;
        public Camera Camera => _camera;
        public GridIndicator GridIndicatorPrefab => _gridIndicatorPrefab;

        public void SetPosition()
        {

        }
    }
}
