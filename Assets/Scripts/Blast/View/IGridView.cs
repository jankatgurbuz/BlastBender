using UnityEngine;

namespace Blast.View
{
    public interface IGridView
    {
        public Grid Grid { get; }
        public Camera Camera { get; }
        public void SetPosition();
    }
}