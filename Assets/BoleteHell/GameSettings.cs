using BoleteHell.AI.Services;
using BoleteHell.Gameplay.Droppables;
using BoleteHell.Gameplay.GameState;
using UnityEngine;
using Zenject;
using Tutorial = BoleteHell.Utils.Advisor.Tutorial;

namespace BoleteHell.Code.Core
{
    [CreateAssetMenu(fileName = "GameSettings", menuName = "BoleteHell/Game Settings")]
    public class GameSettings : ScriptableObjectInstaller
    {
        public DropManager.Config DropManagerConfig;
        public AIDirector.Config SpawnControllerConfig;
        public GameOutcomeService.Config OutcomeConfig;
        public Tutorial.Speakers TutorialSpeakers;
        
        public override void InstallBindings()
        {
            Container.BindInstances(
                DropManagerConfig, 
                SpawnControllerConfig, 
                OutcomeConfig,
                TutorialSpeakers);
        }
    }
}