using System;
using BoleteHell.Code.Core;
using BoleteHell.Code.Gameplay.Characters;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using Zenject;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "SetPlayerReference", story: "Set [player] reference", category: "Action", id: "10a82942bd54f16c43e546426fc17d0f")]
public partial class SetPlayerReferenceAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Player;

    private IEntityFinder _entityFinder;
    
    protected override Status OnStart()
    {
        ServiceLocator.Get(ref _entityFinder);
        Player.Value = _entityFinder.GetPlayer().gameObject;
        return Status.Success;
    }
    
}

