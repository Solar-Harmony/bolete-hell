using BoleteHell.Code.Gameplay.Destructible;
using Zenject;

namespace BoleteHell.Code
{
    public class GameInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<ISpriteFragmenter>().To<SpriteFragmenter>().AsSingle();
            
        }
    }
}