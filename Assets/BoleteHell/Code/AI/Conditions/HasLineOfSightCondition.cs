using System;
using BoleteHell.Code.AI.Services;
using BoleteHell.Code.Core;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;

namespace BoleteHell.Code.AI.Conditions
{
    [Serializable, GeneratePropertyBag]
    [Condition(name: "Has Line of Sight", story: "[Self] has [Agent] in sight, max [ViewRange] meters away", category: "Bolete Hell", id: "619bbda3973eb7d09301865566a2be13")]
    public partial class HasLineOfSightCondition : Condition
    {
        [SerializeReference] public BlackboardVariable<GameObject> Self;
        [SerializeReference] public BlackboardVariable<GameObject> Agent;
        [SerializeReference] public BlackboardVariable<float> ViewRange; // TODO: make param of agent
        
        private ITargetingUtils _targeting;
        
        public override bool IsTrue()
        {
            return _targeting.HasLineOfSight(Self.Value, Agent.Value, ViewRange.Value);
        }

        public override void OnStart()
        {
            ServiceLocator.Get(ref _targeting);
        }
        
        public override void OnEnd()
        {
        }
    }
}
