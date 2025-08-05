using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;

[Serializable, GeneratePropertyBag]
[Condition(name: "Is Not Null", story: "[Object] is not Null", category: "Bolete Hell", id: "db7df5087732cb6e9ec786290eb618cf")]
public partial class IsNotNullCondition : Condition
{
    [SerializeReference] public BlackboardVariable<GameObject> Object;

    public override bool IsTrue()
    {
        return Object.Value;
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
