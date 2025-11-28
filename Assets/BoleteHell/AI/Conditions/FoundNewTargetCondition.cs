using System;
using BoleteHell.AI.Services;
using BoleteHell.Code.Core;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;

namespace BoleteHell.AI.Conditions
{
    [Serializable, GeneratePropertyBag]
    [Condition(
        name: "Closer target available", 
        story: "Closer target is available ➔ [Target]", 
        category: "Bolete Hell", id: "a6c458a36beedbba632aa9f9dc2a4d70")]
    public partial class FoundNewTargetCondition : Condition
    {
        [SerializeReference] public BlackboardVariable<GameObject> Target;

        private IDirector _director;

        public override void OnStart()
        {
            ServiceLocator.Get(out _director);
        }

        public override bool IsTrue()
        {
            GameObject target = _director.FindNearestTarget(GameObject);
            
            if (Target.Value == target) 
                return false;

            Target.Value = target;
            return true;
        }
    }
}
