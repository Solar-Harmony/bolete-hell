using BoleteHell.AI.Services;
using BoleteHell.Gameplay.Droppables;
using BoleteHell.Gameplay.GameState;
using UnityEngine;
using Zenject;

namespace BoleteHell.Code.Core
{
    [CreateAssetMenu(fileName = "GameSettings", menuName = "BoleteHell/Game Settings")]
    public class GameSettings : ScriptableObjectInstaller
    {
        public DropManager.Config DropManagerConfig;
        public Overlord.Config SpawnControllerConfig;
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