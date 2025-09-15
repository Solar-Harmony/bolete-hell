using BoleteHell.Utils;
using UnityEngine;
using Zenject;

namespace Utils.BoleteHell.Utils
{
    public class UtilsInstaller : Installer
    {
        public override void InstallBindings()
        {
            ServiceLocator.Initialize(Container);
            Container.Bind<Camera>().FromInstance(Camera.main).AsSingle();
            Container.Bind<IObjectInstantiator>().To<ObjectInstantiator>().AsSingle();
            Container.Bind<ICoroutineProvider>().To<GlobalCoroutine>().FromNewComponentOnRoot().AsSingle();
        }
    }
}
