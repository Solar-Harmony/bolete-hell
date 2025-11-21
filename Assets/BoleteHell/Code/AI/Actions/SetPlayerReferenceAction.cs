using System;
using BoleteHell.Code.Core;
using BoleteHell.Code.Gameplay.Characters;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

// TODO: I think we can create a Shared Blackboard to get rid of this action
[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Set player reference", story: "Query player, store in [Player]", category: "Action", id: "10a82942bd54f16c43e546426fc17d0f")]
public partial class SetPlayerReferenceAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Player;

    private IEntityFinder _entityFinder;
    
    protected override Status OnStart()
    {
        ServiceLocator.Get(out _entityFinder);
        
        Player.Value = _entityFinder.GetPlayer().gameObject;
        
        return Status.Success;
    }
}

