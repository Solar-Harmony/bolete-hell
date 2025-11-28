using System.Linq;
using BoleteHell.Gameplay.Characters;
using BoleteHell.Gameplay.Characters.Registry;
using BoleteHell.Utils.Extensions;
using UnityEngine;
using Zenject;

namespace BoleteHell.Code.AI.Services
{
    public class Director : IDirector
    {
        [Inject]
        private IEntityRegistry _entityRegistry;
        
        // TODO: Faudrait consolider ca dans le entity finder
        public GameObject FindWeakestAlly(GameObject self)
        {
            return _entityRegistry
                .GetAll(EntityTag.Enemy)
                .Where(e => e != self)
                .Select(e => new { GameObject = e, Health = e.GetComponent<HealthComponent>() })
                .OrderBy(e => e.Health.Percent)
                .FirstOrDefault()?.GameObject; 
        }

        // TODO: Support factions
        public GameObject FindNearestAlly(GameObject self)
        {
            return _entityRegistry
                .GetAll(EntityTag.Enemy)
                .Where(e => e != self)
                .ToList()
                .FindClosestTo(e => e.transform.position, self.transform.position, out float distance);
        }

        public GameObject FindNearestTarget(GameObject self)
        {
            GameObject closestBase = _entityRegistry.GetClosestBase(self.transform.position, out float distanceToClosestBase);
            GameObject player = _entityRegistry.GetPlayer();
            if (!closestBase)
                return player;
            
            float distanceToPlayer = Vector2.Distance(self.transform.position, player.transform.position);
            return distanceToPlayer < distanceToClosestBase ? player : closestBase.gameObject;
        }
    }
}