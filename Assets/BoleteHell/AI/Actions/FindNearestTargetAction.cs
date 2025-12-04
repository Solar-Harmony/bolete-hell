using System;
using BoleteHell.AI.Services;
using BoleteHell.Code.Core;
using BoleteHell.Gameplay.Characters.Enemy;
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
        name: "Find target", 
        story: "Update current target, store in [Target]", 
        category: "Bolete Hell", id: "ab42fd85c68c2ece114cb2058a600000")]
    public class FindNearestTargetAction : Action
    {
        [SerializeReference]
        public BlackboardVariable<GameObject> Target;
        
        private IDirector _director;
        private AIGroupComponent _groupComponent;

        protected override Status OnStart()
        {
            ServiceLocator.Get(out _director);
            GameObject.GetComponentChecked(out _groupComponent);
            
            return Status.Running;
        }

        protected override Status OnUpdate()
        {
            GameObject target = _director.FindTarget(GameObject, _groupComponent.GroupID);

            Target.Value = target;
            return Status.Success;
        }
    }
}