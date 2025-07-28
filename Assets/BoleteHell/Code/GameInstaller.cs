using BoleteHell.Code.Gameplay.Destructible;
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
            if (IsInjected)
                return;
            
            GameInstaller.staticContainer?.Inject(this);
            IsInjected = true;
        }
    }
    
    public class GameInstaller : MonoInstaller
    {
        internal static DiContainer staticContainer;
        
        public override void InstallBindings()
        {
            staticContainer = Container;
            
            Container.Bind<ISpriteFragmenter>().To<SpriteFragmenter>().AsSingle();
            Container.Bind<Camera>().FromInstance(Camera.main).AsSingle();
            
            Container.BindInterfacesAndSelfTo<InputActionsWrapper>().AsSingle();
            Container.BindInterfacesAndSelfTo<InputDispatcher>().AsSingle();

            Container.Bind<ITargetingUtils>().To<TargetingUtils>().AsSingle();
            Container.Bind<IObjectInstantiator>().To<ObjectInstantiator>().AsSingle();
        }
    }
}