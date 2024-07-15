using System.Collections.Generic;
using SC.Core.UI;
using UnityEngine;

namespace Global.Controller
{
    public interface IMainMenuCameraController
    {
        void Register(SpriteCanvas spriteCanvas);
        void AddCamera(Camera camera);
    }

    public class MainMenuCameraController : IMainMenuCameraController
    {
        private List<SpriteCanvas> _spriteCanvasList;
        private List<SpriteCanvas> SpriteCanvasList => _spriteCanvasList ??= new List<SpriteCanvas>();

        public void Register(SpriteCanvas spriteCanvas)
        {
            if (SpriteCanvasList.Contains(spriteCanvas)) return;

            SpriteCanvasList.Add(spriteCanvas);
        }

        public void AddCamera(Camera camera)
        {
            SpriteCanvasList.ForEach((item) => { item.SetCamera(camera); });
        }
    }
}