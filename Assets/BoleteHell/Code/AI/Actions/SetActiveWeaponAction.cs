using System;
using BoleteHell.Code.Arsenal;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Set active weapon", story: "[Agent] uses weapon [Index]", category: "Bolete Hell", id: "9467d778927ea8f1fc1502cb826e23d7")]
public partial class SetActiveWeaponAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<int> Index;

    protected override Status OnStart()
    {
        var arsenal = Agent.Value.GetComponent<Arsenal>();
        if (arsenal)
        {
            arsenal.SetSelectedWeapon(Index.Value);
        }
        
        return Status.Success;
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

