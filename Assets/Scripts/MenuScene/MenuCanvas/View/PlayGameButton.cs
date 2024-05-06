using System.Collections;
using System.Collections.Generic;
using Global.Controller;
using UnityEngine;
using Zenject;

namespace MenuScene.MenuCanvas.View
{
    public class PlayGameButton : MonoBehaviour
    {
        [Inject] private readonly IGameController _gameController;

        public void PlayGame()
        {
            _gameController.GameInitialize();
        }
    }
}
