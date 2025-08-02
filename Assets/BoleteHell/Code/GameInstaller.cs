using BoleteHell.Code.AI.Services;
using BoleteHell.Code.Gameplay.Base;
using BoleteHell.Code.Gameplay.Character;
using BoleteHell.Code.Gameplay.Destructible;
using BoleteHell.Code.Graphics;
using BoleteHell.Code.Input;
using BoleteHell.Code.Utils;
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
        
        // ReSharper disable Unity.PerformanceAnalysis
        public override void InstallBindings()
        {
            staticContainer = Container;
            
            Container.Bind<Camera>().FromInstance(Camera.main).AsSingle();
            
            Container.BindInterfacesAndSelfTo<InputActionsWrapper>().AsSingle();
            Container.BindInterfacesAndSelfTo<InputDispatcher>().AsSingle();

            Container.Bind<ISpriteFragmenter>().To<SpriteFragmenter>().AsSingle();
            Container.Bind<ITargetingUtils>().To<TargetingUtils>().AsSingle();
            Container.Bind<IObjectInstantiator>().To<ObjectInstantiator>().AsSingle();
            Container.Bind<IGlobalCoroutine>().To<GlobalCoroutine>().FromNewComponentOnRoot().AsSingle();
            
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
            
            // temp
            var player = FindFirstObjectByType<Player>();
            Debug.Assert(player);
            Container
                .Bind<ICharacter>()
                .WithId("Player")
                .FromInstance(player);
            
            Container.Bind<IBaseService>().To<BaseService>().AsSingle();
        }
    }
}