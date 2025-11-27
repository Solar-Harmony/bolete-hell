using System;
using BoleteHell.Code.AI.Services;
using BoleteHell.Code.Core;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

namespace BoleteHell.Code.AI.Actions
{
    [Serializable]
    [GeneratePropertyBag]
    [NodeDescription(
        name: "Find nearest target", 
        story: "Find nearest target, store in [Target]", 
        category: "Bolete Hell", id: "ab42fd85c68c2ece114cb2058a600000")]
    public class FindNearestTargetAction : Action
    {
        [SerializeReference]
        public BlackboardVariable<GameObject> Target;
        
        private IDirector _director;

        protected override Status OnStart()
        {
            ServiceLocator.Get(out _director);
            return Status.Running;
        }

        protected override Status OnUpdate()
        {
            GameObject target = _director.FindNearestTarget(GameObject);

            Target.Value = target;
            return Status.Success;
        }
    }
}