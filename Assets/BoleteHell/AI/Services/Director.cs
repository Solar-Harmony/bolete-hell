using System.Linq;
using BoleteHell.AI.Services.Group;
using BoleteHell.Gameplay.Characters;
using BoleteHell.Gameplay.Characters.Enemy;
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

        [Inject]
        private IAIGroupService _groups;
        
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
            return _groups.GetGroup(self.GetComponent<AIGroupComponent>().GroupID).LuiQuilFautButer;
        }
    }
}