using Blast.View;
using UnityEngine;

namespace LevelGenerator.Scripts.View
{
    public class LGGridView : MonoBehaviour, IGridView
    {
        [SerializeField] private Grid _grid;
        [SerializeField] private Camera _camera;

        public Grid Grid => _grid;
        public Camera Camera => _camera;

        public void SetPosition()
        {
        }
    }
}