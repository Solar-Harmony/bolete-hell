using System;
using BoleteHell.AI.Services;
using BoleteHell.Code.Core;
using BoleteHell.Gameplay.Characters.Enemy;
using BoleteHell.Utils.Extensions;
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
        private AIGroupComponent _groupComponent;

        public override void OnStart()
        {
            ServiceLocator.Get(out _director);
            GameObject.GetComponentChecked(out _groupComponent);
        }

        public override bool IsTrue()
        {
            GameObject target = _director.FindTarget(GameObject, _groupComponent.GroupID);
            
            if (Target.Value == target) 
                return false;

            Target.Value = target;
            return true;
        }
    }
}
