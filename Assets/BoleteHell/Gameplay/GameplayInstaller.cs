using System.Linq;
using BoleteHell.Gameplay.Base;
using BoleteHell.Gameplay.Character;
using BoleteHell.Gameplay.Damage.Effects;
using BoleteHell.Gameplay.Destructible;
using BoleteHell.Gameplay.GameState;
using BoleteHell.Gameplay.Graphics;
using BoleteHell.Gameplay.Input;
using Sirenix.Utilities;
using UnityEngine;
using Zenject;

namespace BoleteHell.Gameplay
{
    public class GameplayInstaller : Installer
    {
        private readonly GameObject _transientLightPrefab;
        private readonly GameObject _spriteFragmentPrefab;

        public GameplayInstaller(GameObject transientLightPrefab, GameObject spriteFragmentPrefab)
        {
            _transientLightPrefab = transientLightPrefab;
            _spriteFragmentPrefab = spriteFragmentPrefab;
        }

        public override void InstallBindings()
        {
            // gameplay services
            Container.Bind<IGameOutcomeService>().To<GameOutcomeService>().AsSingle();
            Container.Bind<IBaseService>().To<BaseService>().AsSingle();
            Container.Bind<IEntityFinder>().To<EntityFinder>().FromNewComponentOnRoot().AsSingle();
            Container.Bind<ISpriteFragmenter>().To<SpriteFragmenter>().AsSingle();
            
            BindStatusEffects();
            
            // input
            Container.BindInterfacesAndSelfTo<InputActionsWrapper>().AsSingle();
            Container.BindInterfacesAndSelfTo<InputDispatcher>().AsSingle();
            Container.Bind<IInputState>().To<InputState>().AsSingle();
            
            // pools
            Container.BindMemoryPool<TransientLight, TransientLight.Pool>()
                .WithInitialSize(10)
                .WithMaxSize(50)
                .ExpandByOneAtATime()
                .FromComponentInNewPrefab(_transientLightPrefab)
                .UnderTransformGroup("TransientLights");
            Container.BindMemoryPool<SpriteFragment, SpriteFragment.Pool>()
                .WithInitialSize(30)
                .WithMaxSize(250)
                .ExpandByDoubling()
                .FromComponentInNewPrefab(_spriteFragmentPrefab)
                .UnderTransformGroup("SpriteFragments");
        }

        private void BindStatusEffects()
        {
            typeof(IStatusEffect).Assembly
                .GetTypes()
                .Where(t => typeof(IStatusEffect).IsAssignableFrom(t) && !t.IsAbstract && !t.IsInterface)
                .ForEach(type =>
                { 
                    Container.Bind<IStatusEffect>().To(type).AsSingle();
                });
            
            Container.BindInterfacesTo<StatusEffectService>().AsSingle();
        }
    }
}
