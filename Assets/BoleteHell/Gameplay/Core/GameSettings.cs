using BoleteHell.Gameplay.Droppables;
using BoleteHell.Gameplay.GameState;
using BoleteHell.Gameplay.SpawnManager;
using UnityEngine;
using Zenject;

namespace BoleteHell.Code.Core
{
    [CreateAssetMenu(fileName = "GameSettings", menuName = "BoleteHell/Game Settings")]
    public class GameSettings : ScriptableObjectInstaller
    {
        public DropManager.Config DropManagerConfig;
        public SpawnController.Config SpawnControllerConfig;
        public GameOutcomeService.Config OutcomeConfig;
        
        public override void InstallBindings()
        {
            Container.BindInstances(
                DropManagerConfig, 
                SpawnControllerConfig, 
                OutcomeConfig);
        }
    }
}