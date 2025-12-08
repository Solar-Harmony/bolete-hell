using System;
using System.Collections.Generic;
using System.Linq;
using BoleteHell.AI.Services;
using BoleteHell.AI.Services.Group;
using BoleteHell.Audio;
using BoleteHell.Code.Arsenal.Cannons;
using BoleteHell.Code.Arsenal.Shields;
using BoleteHell.Code.Arsenal.ShotPatterns;
using BoleteHell.Code.Gameplay.Damage.Effects;
using BoleteHell.Code.Gameplay.Input;
using BoleteHell.Code.Graphics;
using BoleteHell.Code.Input;
using BoleteHell.Gameplay.Characters.Enemy;
using BoleteHell.Gameplay.Characters.Enemy.Factory;
using BoleteHell.Gameplay.Characters.Registry;
using BoleteHell.Gameplay.Destructible;
using BoleteHell.Gameplay.Droppables;
using BoleteHell.Gameplay.GameState;
using BoleteHell.Gameplay.InputControllers;
using BoleteHell.Gameplay.SpawnManager;
using BoleteHell.Rendering.Ripples;
using BoleteHell.Utils;
using BoleteHell.Utils.Advisor;
using UnityEngine;
using Zenject;

namespace BoleteHell.Code.Core
{
    // TODO: Split this into multiple installers
    public class GameInstaller : MonoInstaller
    {
        // TODO: This can be moved to per service settings
        [SerializeField]
        private GameObject spriteFragmentPrefab;
        
        [SerializeField]
        private GameObject transientLightPrefab;

        [SerializeField]
        private GameObject healthTextPrefab;

        [SerializeField]
        private GameObject shieldPreviewPrefab;
        
        [SerializeField]
        private GameObject laserPreviewPrefab;

        // ReSharper disable Unity.PerformanceAnalysis
        public override void InstallBindings()
        {
            ServiceLocator.Initialize(Container);
            
            // gameplay
            Container.Bind<IGameOutcomeService>().To<GameOutcomeService>().AsSingle();
            Container.Bind<ICannonService>().To<CannonService>().AsSingle();
            Container.Bind<IShotPatternService>().To<ShotPatternService>().AsSingle();
            Container.Bind<IAudioPlayer>().To<AudioPlayer>().AsSingle();
            Container.Bind<IEntityRegistry>().To<EntityRegistry>().FromNewComponentOnRoot().AsSingle();
            Container.BindInterfacesAndSelfTo<SpawnService>().AsSingle();
            Container.Bind<AIDirector>().FromComponentInHierarchy().AsSingle();
            Container.Bind<CreepManager>().FromComponentInHierarchy().AsSingle();
            Container.Bind<RippleManager>().FromComponentInHierarchy().AsSingle();
            Container.Bind<IAIGroupService>().To<AIGroupService>().AsSingle();
            Container.Bind<IDropManager>().To<DropManager>().AsSingle();
            Container.Bind<ITutorialService>().To<Tutorial>().FromComponentInHierarchy().AsSingle();
            BindStatusEffects();
            
            // factories
            Container.BindFactory<GameObject, ShieldData, ShieldPreviewDrawer, ShieldPreviewFactory>()
                .FromComponentInNewPrefab(shieldPreviewPrefab);
            
            // input
            Container.BindInterfacesAndSelfTo<InputActionsWrapper>().AsSingle();
            Container.BindInterfacesAndSelfTo<InputDispatcher>().AsSingle();
            Container.Bind<IInputState>().To<InputState>().AsSingle();
            Container.Bind<DebugInput>().FromComponentInHierarchy().AsSingle();

            // utils
            Container.Bind<Camera>().FromInstance(Camera.main).AsSingle();
            Container.Bind<ISpriteFragmenter>().To<SpriteFragmenter>().AsSingle();
            Container.Bind<ITargetingUtils>().To<TargetingUtils>().AsSingle();
            Container.Bind<IObjectInstantiator>().To<ObjectInstantiator>().AsSingle();
            Container.Bind<ICoroutineProvider>().To<GlobalCoroutine>().FromNewComponentOnRoot().AsSingle();
            
            // pools
            Container.BindInterfacesAndSelfTo<EnemyPool>().AsSingle();
            
            Container.BindMemoryPool<TransientLight, TransientLight.Pool>()
                .WithInitialSize(10)
                .WithMaxSize(50)
                .ExpandByOneAtATime()
                .FromComponentInNewPrefab(transientLightPrefab)
                .UnderTransformGroup("TransientLights"); 
            
            Container.BindMemoryPool<LaserPreviewRenderer, LaserPreviewRenderer.Pool>()
                .WithInitialSize(30)
                .WithMaxSize(250)
                .ExpandByOneAtATime()
                .FromComponentInNewPrefab(laserPreviewPrefab)
                .UnderTransformGroup("LaserPreviews");
            
            Container.BindMemoryPool<SpriteFragment, SpriteFragment.Pool>()
                .WithInitialSize(30)
                .WithMaxSize(250)
                .ExpandByDoubling()
                .FromComponentInNewPrefab(spriteFragmentPrefab)
                .UnderTransformGroup("SpriteFragments");
            
            Container.BindMemoryPool<HealthText, HealthText.Pool>()
                .WithInitialSize(30)
                .WithMaxSize(250)
                .ExpandByDoubling()
                .FromComponentInNewPrefab(healthTextPrefab)
                .UnderTransformGroup("HealthTexts");
        }

        private void BindStatusEffects()
        {
            IEnumerable<Type> types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(asm => asm.GetTypes())
                .Where(t => typeof(IStatusEffect).IsAssignableFrom(t) && !t.IsAbstract && !t.IsInterface);
                
            foreach (var type in types)
            { 
                Container.Bind<IStatusEffect>().To(type).AsSingle();
            }
            
            Container.BindInterfacesTo<StatusEffectService>().AsSingle();
        }
    }
}