using BoleteHell.AI.Services;
using Zenject;

namespace BoleteHell.AI
{
    public class AIInstaller : Installer
    {
        public override void InstallBindings()
        {
            Container.Bind<ITargetingUtils>().To<TargetingUtils>().AsSingle();
            Container.Bind<IDirector>().To<Director>().AsSingle();
        }
    }
}
