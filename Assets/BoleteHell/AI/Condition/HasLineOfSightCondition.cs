using System;
using BoleteHell.AI.Boilerplate;
using BoleteHell.AI.Services;
using BoleteHell.Utils;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Zenject;

namespace BoleteHell.AI.Condition
{
    [Serializable, GeneratePropertyBag]
    [Condition(name: "Has Line of Sight", story: "[Self] has line of sight [ViewRange] for [Agent]", category: "Bolete Hell", id: "619bbda3973eb7d09301865566a2be13")]
    public partial class HasLineOfSightCondition : BoleteCondition
    {
        [SerializeReference] public BlackboardVariable<GameObject> Self;
        [SerializeReference] public BlackboardVariable<GameObject> Agent;
        [SerializeReference] public BlackboardVariable<float> ViewRange; // todo make param of agent
        
        [Inject]
        private ITargetingUtils _targeting;
        
        public override bool IsTrueImpl()
        {
            return _targeting.HasLineOfSight(Self.Value, Agent.Value, ViewRange.Value);
        }

        public override void OnStart()
        {
            ServiceLocator.Inject(this);
        }

        public override void OnEnd()
        {
        }
    }
}
