using BoleteHell.Arsenals.Cannons;
using BoleteHell.Arsenals.Shields;
using BoleteHell.Arsenals.ShotPatterns;
using Zenject;

namespace BoleteHell.Arsenals
{
    public class ArsenalInstaller : Installer
    {
        public override void InstallBindings()
        {
            Container.Bind<ICannonService>().To<CannonService>().AsSingle();
            Container.Bind<IShotPatternService>().To<ShotPatternService>().AsSingle();
            Container.BindFactory<ShieldData, ShieldPreviewDrawer, ShieldPreviewFactory>().AsSingle();
        }
    }
}
