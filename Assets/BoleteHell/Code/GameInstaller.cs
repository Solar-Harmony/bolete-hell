using System.Linq;
using BoleteHell.Code.AI.Services;
using BoleteHell.Code.Arsenal.Cannons;
using BoleteHell.Code.Arsenal.Shields;
using BoleteHell.Code.Arsenal.ShotPatterns;
using BoleteHell.Code.Audio;
using BoleteHell.Code.Gameplay.Base;
using BoleteHell.Code.Gameplay.Character;
using BoleteHell.Code.Gameplay.Damage.Effects;
using BoleteHell.Code.Gameplay.Destructible;
using BoleteHell.Code.Gameplay.GameState;
using BoleteHell.Code.Gameplay.Input;
using BoleteHell.Code.Graphics;
using BoleteHell.Code.Input;
using BoleteHell.Code.Utils;
using Sirenix.Utilities;
using UnityEngine;
using Zenject;

namespace BoleteHell.Code
{
    /// <summary>
    /// Exposes a method to manually request dependency injection into self.
    /// Required in places that don't support proper injection, e.g.: BehaviorTree nodes, SerializedReference objects.
    /// Not a great workaround but it's better than calling static classes.
    /// </summary>
    public interface IRequestManualInject
    {
        protected bool IsInjected { get; set; }
        
        public void InjectDependencies()
        {
            // FIXME: This fails for certain dependencies
            // if (IsInjected)
            //     return; 
            
            GameInstaller.StaticContainer?.Inject(this);
            IsInjected = true;
        }
    }
    
    public class GameInstaller : MonoInstaller
    {
        // TODO: This should not be internal lol
        internal static DiContainer StaticContainer;
        
        // TODO: This can be moved to per service settings
        [SerializeField]
        private GameObject spriteFragmentPrefab;
        
        [SerializeField]
        private GameObject transientLightPrefab;
        
        [SerializeField]
        private GameObject shieldPreviewPrefab;
        
        // TODO: Split this into multiple installers
        // This will require splitting out code into modules though
        // Which is a good thing but it's also cancer to do at first lol
        // ReSharper disable Unity.PerformanceAnalysis
        public override void InstallBindings()
        {
            StaticContainer = Container;

            // the player (temp?)
            var player = FindFirstObjectByType<Player>();
            Debug.Assert(player);
            Container
                .Bind<ISceneObject>()
                .WithId("Player")
                .FromInstance(player);
            
            // gameplay
            Container.Bind<IGameOutcomeService>().To<GameOutcomeService>().AsSingle();
            Container.Bind<IDirector>().To<Director>().AsSingle();
            Container.Bind<ICannonService>().To<CannonService>().AsSingle();
            Container.Bind<IShotPatternService>().To<ShotPatternService>().AsSingle();
            Container.Bind<IAudioPlayer>().To<AudioPlayer>().AsSingle();
            Container.Bind<IBaseService>().To<BaseService>().AsSingle();
            Container.Bind<IEntityFinder>().To<EntityFinder>().FromNewComponentOnRoot().AsSingle();
            BindStatusEffects();
            
            // factories
            Container.BindFactory<Character, ShieldData, ShieldPreviewDrawer, ShieldPreviewFactory>()
                .FromComponentInNewPrefab(shieldPreviewPrefab);
            
            // input
            Container.BindInterfacesAndSelfTo<InputActionsWrapper>().AsSingle();
            Container.BindInterfacesAndSelfTo<InputDispatcher>().AsSingle();
            Container.Bind<IInputState>().To<InputState>().AsSingle();

            // utils
            Container.Bind<Camera>().FromInstance(Camera.main).AsSingle();
            Container.Bind<ISpriteFragmenter>().To<SpriteFragmenter>().AsSingle();
            Container.Bind<ITargetingUtils>().To<TargetingUtils>().AsSingle();
            Container.Bind<IObjectInstantiator>().To<ObjectInstantiator>().AsSingle();
            Container.Bind<ICoroutineProvider>().To<GlobalCoroutine>().FromNewComponentOnRoot().AsSingle();
            
            // pools
            Container.BindMemoryPool<TransientLight, TransientLight.Pool>()
                .WithInitialSize(10)
                .WithMaxSize(50)
                .ExpandByOneAtATime()
                .FromComponentInNewPrefab(transientLightPrefab)
                .UnderTransformGroup("TransientLights");
            Container.BindMemoryPool<SpriteFragment, SpriteFragment.Pool>()
                .WithInitialSize(30)
                .WithMaxSize(250)
                .ExpandByDoubling()
                .FromComponentInNewPrefab(spriteFragmentPrefab)
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