using BoleteHell.Code.Gameplay.Damage;
using BoleteHell.Code.Gameplay.GameState;
using Unity.Behavior;
using UnityEngine;
using Zenject;

namespace BoleteHell.Code.Gameplay.Characters
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(BehaviorGraphAgent))]
    public class AIComponent : MonoBehaviour
    {
        [Inject]
        private IGameOutcomeService _outcome;
        
        private BehaviorGraphAgent _agent;
        private HealthComponent _health;
        
        private void Awake()
        {
            _agent = GetComponent<BehaviorGraphAgent>();
            _health = GetComponent<HealthComponent>();
        }

        private void OnEnable()
        {
            _outcome.OnDefeat += OnDefeat;
            _health.OnDeath += OnDefeat;
        }
        
        private void OnDefeat()
        {
            Disable();
        }

        private void OnDefeat(string reason)
        {
            Disable();
        }

        private void Disable()
        {
            _agent.enabled = false;
        }
        
        private void OnDisable()
        {
            _outcome.OnDefeat -= OnDefeat;
        }
    }
}