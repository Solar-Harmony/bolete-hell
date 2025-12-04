using BoleteHell.Utils.Advisor;
using UnityEngine;
using Zenject;

namespace BoleteHell.Gameplay.Characters.Enemy
{
    [RequireComponent(typeof(HealthComponent))]
    public class AchievementKillFirstEnemy : MonoBehaviour
    {
        [Inject]
        private ITutorialService _tutorial;
        
        private static bool _complete = false;

        private void Awake()
        {
            if (_complete)
            {
                enabled = false;
                return;
            }
            
            var health = GetComponent<HealthComponent>();
            health.OnDeath += OnDeath;
        }
        
        private void OnDeath()
        {
            if (_complete)
                return;
            
            _tutorial.ShowTutorial(TutorialEvent.KilledFirstBolete);
            _complete = true;
        }
    }
}