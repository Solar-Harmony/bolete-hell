using System.Linq;
using BoleteHell.Gameplay.Characters;
using BoleteHell.Gameplay.Characters.Registry;
using BoleteHell.Utils.Extensions;
using UnityEngine;
using Zenject;

namespace BoleteHell.AI.Services
{
    public class Director : IDirector
    {
        [Inject]
        private IEntityRegistry _entities;
        
        public GameObject FindWeakestAlly(GameObject self)
        {
            return _entities
                .WithTag(EntityTag.Enemy)
                .TakeBest((HealthComponent h) => h.Percent);
        }

        public GameObject FindNearestAlly(GameObject self)
        {
            return _entities
                .WithTag(EntityTag.Enemy)
                .Where(e => e != self)
                .TakeClosestTo(self, out _);
        }

        public GameObject FindNearestTarget(GameObject self)
        {
            GameObject closestBase = _entities.GetClosestBase(self.transform.position, out float distanceToClosestBase);
            GameObject player = _entities.GetPlayer();
            if (!closestBase)
                return player;
            
            float distanceToPlayer = Vector2.Distance(self.transform.position, player.transform.position);
            return distanceToPlayer < distanceToClosestBase ? player : closestBase.gameObject;
        }
    }
}