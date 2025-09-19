using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "IsLowOnHealth", story: "[Agent] health is under [treshold]", category: "Bolete Hell", id: "98477ce1a70f4bceef2bc38eac76b271")]
public partial class IsLowOnHealthCondition : Condition
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<int> Treshold;

    public override bool IsTrue()
    {
        return true;
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
