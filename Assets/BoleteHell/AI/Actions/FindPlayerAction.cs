using System;
using BoleteHell.Code.Core;
using BoleteHell.Gameplay.Characters.Registry;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "FindPlayer", story: "Find player ➔ [target]", category: "Action", id: "81942314c3dc660a552e4c4ed15646ea")]
public partial class FindPlayerAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Target;

    private IEntityRegistry _entities;
    
    protected override Status OnStart()
    {
        ServiceLocator.Get(out _entities);
        Target.Value = _entities.GetPlayer();
        Debug.Log("Set Player");
        return Status.Success;
    }
    
}

