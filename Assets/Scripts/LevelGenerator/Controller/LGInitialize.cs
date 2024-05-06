using System.Collections;
using System.Collections.Generic;
using Blast.Controller;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace LevelGenerator.Controller
{
    public interface ILGStart
    {
        public void Start();
    }
    public class LGInitialize : IInitializable
    {
        private readonly DiContainer _container;

        public LGInitialize(DiContainer container)
        {
            _container = container;
        }
        public void Initialize()
        {
            _container.Resolve<ILGGridController>().Start();
            _container.Resolve<ILevelGeneratorController>().Start();
            _container.Resolve<ILGGridInteractionController>().Start();
        }
    }
}
