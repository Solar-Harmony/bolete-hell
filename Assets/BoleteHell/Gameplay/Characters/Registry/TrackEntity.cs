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
        
        [SerializeField]
        private EntityTag[] _tags = Array.Empty<EntityTag>();

        private HealthComponent _health;
        private bool _unregistered = false;

        private void Awake()
        {
            _health = GetComponent<HealthComponent>();
        }
        
        private void OnEnable()
        {
            _unregistered = false;
            _registry.Register(_tags, gameObject);
            
            if (_health)
            {
                _health.OnDeath += OnDeath;
            }
        }

        private void OnDeath()
        {
            Unregister();
        }
        
        // This triggers:
        // - When obj.SetActive(false) is called
        // - When Destroy(obj) is called
        // - When the scene is unloaded
        // Since we destroy objects upon death, it also works when an entity dies.
        // TODO: If we ever implement Enemy NPC pooling, we will need to adjust this.
        private void OnDisable()
        {
            Unregister();
            
            if (_health)
            {
                _health.OnDeath -= OnDeath;
            }
        }

        private void Unregister()
        {
            if (_unregistered)
                return;
            
            _unregistered = true;
            _registry.Unregister(_tags, gameObject);
        }
    }
}