using System;
using System.Linq;
using BoleteHell.Code.Core;
using BoleteHell.Gameplay.Characters;
using BoleteHell.Gameplay.Characters.Registry;
using BoleteHell.Utils.Extensions;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

namespace BoleteHell.AI.Actions
{
    [Serializable]
    [GeneratePropertyBag]
    [NodeDescription(
        name: "Find unhealthiest ally", 
        story: "Find unhealthiest ally, store in [CurrentTarget]", 
        category: "Bolete Hell", id: "ab42fd85c68c2ece114cb2058a600001")]
    public class FindWeakestAlly : Action
    {
        [SerializeReference]
        public BlackboardVariable<GameObject> CurrentTarget;
        
        private IEntityRegistry _entities;

        protected override Status OnStart()
        {
            ServiceLocator.Get(out _entities);
             
            return Status.Running;
        }

        protected override Status OnUpdate()
        {
            GameObject target = _entities
                .WithTag(EntityTag.Enemy).ToList()
                .OrderBy(e => e.GetComponent<HealthComponent>().Percent)
                .FirstOrDefault();

            
            if (!target)
            {
                CurrentTarget.Value = null;
                return Status.Success;
            }
            
            var faction = target.GetComponent<FactionComponent>();
            var health = target.GetComponent<HealthComponent>();
            
            CurrentTarget.Value = faction.Type == FactionType.Enemy && health.Percent < 1.0f
                ? target
                : null;

            return Status.Success;
        }
    }
}