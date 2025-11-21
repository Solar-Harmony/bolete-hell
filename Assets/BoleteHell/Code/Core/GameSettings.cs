using BoleteHell.Code.Gameplay.Base;
using BoleteHell.Code.Gameplay.Droppables;
using BoleteHell.Code.Gameplay.SpawnManager;
using UnityEngine;
using Zenject;

namespace BoleteHell.Code.Core
{
    [CreateAssetMenu(fileName = "GameSettings", menuName = "BoleteHell/Game Settings")]
    public class GameSettings : ScriptableObjectInstaller
    {
        public DropManager.Config DropManagerConfig;
        public SpawnController.Config SpawnControllerConfig;
        public BaseService.Config BaseServiceConfig;
        
        public override void InstallBindings()
        {
            Container.BindInstances(
                DropManagerConfig, 
                SpawnControllerConfig, 
                BaseServiceConfig);
        }
    }
}