using UnityEngine;

namespace Blast.View
{
    public class GridView : MonoBehaviour, IGridView
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