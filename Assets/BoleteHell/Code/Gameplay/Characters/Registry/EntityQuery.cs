using System;
using System.Collections.Generic;
using System.Linq;
using BoleteHell.Code.Gameplay.Damage;
using BoleteHell.Utils.Extensions;
using UnityEngine;

namespace BoleteHell.Code.Gameplay.Characters.Registry
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

        public List<GameObject> GetAll(EntityTag ttag)
        {
            return _entities[ttag];
        }

        public int GetCount(EntityTag ttag)
        {
            return _entities[ttag].Count;
        }

        public GameObject GetWeakestEliteAlive()
        {
            return _entities[EntityTag.EliteEnemy] 
                .Select(e => new { Entity = e, Health = e.GetComponent<HealthComponent>() })
                .Where(e => e.Health.CurrentHealth > 0)
                .OrderBy(e => e.Health.CurrentHealth)
                .FirstOrDefault()?.Entity;
        }
        
        public GameObject GetClosestBase(Vector2 pos, out float distance)
        {
            return _entities[EntityTag.Base]
                .FindClosestTo(b => b.transform.position, pos, out distance);
        }

        public GameObject GetWeakestBase()
        {
            return _entities[EntityTag.Base]
                .Select(b => new { Base = b, Health = b.GetComponent<HealthComponent>() })
                .OrderBy(b => b.Health.CurrentHealth)
                .FirstOrDefault()?.Base;
        }

        public void Register(EntityTag[] tags, GameObject entity)
        {
            foreach (EntityTag type in tags)
            {
                if (type == EntityTag.Player && _entities[type].Count > 0)
                {
                    throw new InvalidOperationException("Multiple player entities registered.");
                }
                
                _entities[type].Add(entity);
            }
        }

        public void Unregister(EntityTag[] tags, GameObject entity)
        {
            foreach (EntityTag type in tags)
            {
                _entities[type].Remove(entity);
            }
            
            EntityDied?.Invoke((tags, entity));
        }
    }
}