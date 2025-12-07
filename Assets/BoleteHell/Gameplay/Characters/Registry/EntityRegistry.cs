using System;
using System.Collections.Generic;
using System.Linq;
using BoleteHell.Utils.Extensions;
using UnityEngine;

namespace BoleteHell.Gameplay.Characters.Registry
{
    public class EntityRegistry : MonoBehaviour, IEntityRegistry
    {
        private readonly Dictionary<EntityTag, List<GameObject>> _entities = new()
        {
            { EntityTag.Player, new List<GameObject>() },
            { EntityTag.Enemy, new List<GameObject>() },
            { EntityTag.EliteEnemy, new List<GameObject>() },
            { EntityTag.Base, new List<GameObject>() }
        };
        
        public event Action<(EntityTag[], GameObject)> EntityDied;

        public GameObject GetPlayer()
        { 
            return _entities[EntityTag.Player].SingleOrDefault(); 
        }

        public IEnumerable<GameObject> WithTag(EntityTag entityTag)
        {
            return _entities[entityTag];
        }
        
        public int GetCount(EntityTag entityTag)
        {
            return _entities[entityTag].Count;
        }

        public GameObject GetWeakestEliteAlive()
        {
            return WithTag(EntityTag.EliteEnemy)
                .WithLowest((HealthComponent h) => h.CurrentHealth);
        }
        
        public GameObject GetClosestBase(Vector2 pos, out float distance)
        {
            return WithTag(EntityTag.Base)
                .TakeClosestTo(b => b.transform.position, pos, out distance);
        }

        public GameObject GetWeakestBase()
        {
            return WithTag(EntityTag.Base)
                .WithLowest((HealthComponent h) => h.CurrentHealth);
        }

        public void Register(EntityTag[] tags, GameObject entity)
        {
            foreach (EntityTag type in tags)
            {
                if (type == EntityTag.Player && _entities[type].Count > 0)
                {
                    throw new InvalidOperationException("Multiple player entities registered.");
                }
                
                Debug.AssertFormat(!_entities[type].Contains(entity), "Entity '{0}' is already registered!", entity.gameObject.name);
                
                _entities[type].Add(entity);
            }
        }

        public void Unregister(EntityTag[] tags, GameObject entity)
        {
            foreach (EntityTag type in tags)
            {
                _entities[type].RemoveAll(e => ReferenceEquals(e, entity));
            }
            
            EntityDied?.Invoke((tags, entity));
        }
    }
}