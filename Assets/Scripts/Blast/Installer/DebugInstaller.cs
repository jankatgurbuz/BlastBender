using Blast.Controller;
using UnityEngine;
using Zenject;

namespace Blast.Installer
{
    public class DebugInstaller : MonoInstaller<InGameInstaller>
    {
        [SerializeField] private bool _enableDebug;

        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<DebugController>().AsSingle().WithArguments(_enableDebug).NonLazy();
        }
    }
}