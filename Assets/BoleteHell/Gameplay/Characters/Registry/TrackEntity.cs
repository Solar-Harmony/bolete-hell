using System;
using UnityEngine;
using Zenject;

namespace BoleteHell.Gameplay.Characters.Registry
{
    /// <summary>
    /// Registers an entity to be tracked by other systems.
    /// Inject IEntityRegistry to access them.
    /// </summary>
    [DisallowMultipleComponent]
    public class TrackEntity : MonoBehaviour
    {
        [Inject]
        private IEntityRegistry _registry;
        
        private HealthComponent _health;

        [SerializeField]
        private EntityTag[] _tags = Array.Empty<EntityTag>();
        
        private void Awake()
        {
            _health = GetComponent<HealthComponent>();
        }
        
        private void OnEnable()
        {
            _registry.Register(_tags, gameObject);
            _health.OnDeath += OnDeath;
        }
        
        private void OnDeath()
        {
            _registry.Unregister(_tags, gameObject);
        }
        
        private void OnDisable()
        {
            _registry.Unregister(_tags, gameObject);
            _health.OnDeath -= OnDeath;
        }
    }
}