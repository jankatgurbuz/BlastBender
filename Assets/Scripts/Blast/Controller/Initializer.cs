using System;
using Cysharp.Threading.Tasks;
using Global.Controller;
using UnityEngine;
using Zenject;

namespace Blast.Controller
{
    public class Initializer : IInitializable
    {
        private readonly DiContainer _diContainer;
        private readonly IGameController _gameController;

        public Initializer(DiContainer diContainer, IGameController gameController)
        {
            _diContainer = diContainer;
            _gameController = gameController;
        }

        public async void Initialize()
        {
            Application.targetFrameRate = 60;

            var controllers = _diContainer.ResolveAll<IStartable>();

            foreach (var item in controllers)
            {
                await item.Start();
            }
            await UniTask.Delay(TimeSpan.FromSeconds(0.1f));

            await _gameController.MenuInitialize();
            _gameController.InitializationComplete = true;
        }
    }
}
