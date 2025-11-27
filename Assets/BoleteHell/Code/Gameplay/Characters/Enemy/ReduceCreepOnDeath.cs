using BoleteHell.Code.Gameplay.Damage;
using BoleteHell.Code.Graphics;
using UnityEngine;
using Zenject;

namespace BoleteHell.Code.Gameplay.Characters.Enemy
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(HealthComponent))]
    public class ReduceCreepOnDeath : MonoBehaviour
    {
        [Inject]
        private CreepManager _creep;
        
        private HealthComponent _health;
        
        private void Awake()
        {
            _health = GetComponent<HealthComponent>();
        }
        
        private void OnEnable()
        {
            _health.OnDeath += OnDeath;
        }
        
        private void OnDeath()
        {
            _creep.SpreadLevel -= 0.01f;
        }
        
        private void OnDisable()
        {
            _health.OnDeath -= OnDeath;
        }
    }
}