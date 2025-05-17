using System;
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
        if (Self.Value == null || Agent.Value == null)
            return false;
        
        Vector3 direction = Agent.Value.transform.position - Self.Value.transform.position;
        LayerMask layerMask = ~LayerMask.GetMask("PlayerEnemy");
        RaycastHit2D hit = Physics2D.Raycast(Self.Value.transform.position, direction.normalized, ViewRange.Value, layerMask);

        return hit.collider.gameObject == Agent.Value;
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
