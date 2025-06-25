using System;
using _BoleteHell.Code.AI;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "Has Line of Sight", story: "[Self] has line of sight [ViewRange] for [Agent]", category: "Bolete Hell", id: "619bbda3973eb7d09301865566a2be13")]
public partial class HasLineOfSightCondition : Condition
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<float> ViewRange; // todo make param of agent

    public override bool IsTrue()
    {
        return AIUtils.HasLineOfSight(Self.Value, Agent.Value, ViewRange.Value);
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
