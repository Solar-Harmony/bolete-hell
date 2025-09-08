using System.Linq;
using BoleteHell.Code.AI.Services;
using BoleteHell.Code.Audio;
using BoleteHell.Code.Gameplay.Base;
using BoleteHell.Code.Gameplay.Character;
using BoleteHell.Code.Gameplay.Damage.Effects;
using BoleteHell.Code.Gameplay.Destructible;
using BoleteHell.Code.Gameplay.GameState;
using BoleteHell.Code.Gameplay.Input;
using BoleteHell.Code.Graphics;
using BoleteHell.Code.Input;
using BoleteHell.Code.UI;
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
            // TODO: I dont remember why this is commented lmao
            // if (IsInjected)
            //     return; 
            
            GameInstaller.staticContainer?.Inject(this);
            IsInjected = true;
        }
    }
    
    public class GameInstaller : MonoInstaller
    {
        // TODO: This should not be internal lol
        internal static DiContainer staticContainer;
        
        // TODO: This can be moved to per service settings
        [SerializeField]
        private GameObject spriteFragmentPrefab;
        
        [SerializeField]
        private GameObject transientLightPrefab;
        
        // TODO: Split this into multiple installers
        // This will require splitting out code into modules though
        // Which is a good thing but it's also cancer to do at first lol
        // ReSharper disable Unity.PerformanceAnalysis
        public override void InstallBindings()
        {
            staticContainer = Container;
            
            Container.Bind<Camera>().FromInstance(Camera.main).AsSingle();
            
            Container.BindInterfacesAndSelfTo<InputActionsWrapper>().AsSingle();
            Container.BindInterfacesAndSelfTo<InputDispatcher>().AsSingle();
            Container.Bind<IInputState>().To<InputState>().AsSingle();

            Container.Bind<ISpriteFragmenter>().To<SpriteFragmenter>().AsSingle();
            Container.Bind<ITargetingUtils>().To<TargetingUtils>().AsSingle();
            Container.Bind<IObjectInstantiator>().To<ObjectInstantiator>().AsSingle();
            Container.Bind<ICoroutineProvider>().To<GlobalCoroutine>().FromNewComponentOnRoot().AsSingle();
            Container.Bind<IGameOutcomeService>().To<GameOutcomeService>().AsSingle();
            Container.Bind<IDirector>().To<Director>().AsSingle();
            
            Container.Bind<VictoryScreen>()
                .FromComponentInNewPrefabResource("UI/VictoryScreen")
                .UnderTransformGroup("UI")
                .AsSingle()
                .NonLazy();
            
            BindMemoryPools();

            // temp
            var player = FindFirstObjectByType<Player>();
            Debug.Assert(player);
            Container
                .Bind<ISceneObject>()
                .WithId("Player")
                .FromInstance(player);
            
            Container.Bind<IBaseService>().To<BaseService>().AsSingle();

            BindStatusEffects();
            Container.BindInterfacesTo<StatusEffectService>().AsSingle();
            
            Container.Bind<IAudioPlayer>().To<AudioPlayer>().AsSingle();
        }

        private void BindMemoryPools()
        {
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
        }
    }
}