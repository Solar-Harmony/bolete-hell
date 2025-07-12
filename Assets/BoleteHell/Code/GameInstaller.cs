using BoleteHell.Code.Gameplay.Destructible;
using BoleteHell.Code.Input;
using BoleteHell.Code.Input.Controllers;
using UnityEngine;
using Zenject;

namespace BoleteHell.Code
{
    public class GameInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<ISpriteFragmenter>().To<SpriteFragmenter>().AsSingle();
            Container.Bind<Camera>().FromInstance(Camera.main).AsSingle();
            
            Container.BindInterfacesAndSelfTo<InputActionsWrapper>().AsSingle();
            Container.BindInterfacesAndSelfTo<InputDispatcher>().AsSingle();
        }
    }
}