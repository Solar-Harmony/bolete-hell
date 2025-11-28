using System;
using BoleteHell.Code.Gameplay.Characters;
using BoleteHell.Utils.Extensions;
using Pathfinding;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

[Serializable, GeneratePropertyBag]
[NodeDescription(
    name: "Pathfind to position", 
    story: "Pathfind towards [Position]", 
    category: "Bolete Hell", id: "4d6c55f8d197383e9b5f4ea442dce9bb")]
public partial class PathfindToPositionAction : Action
{
    [SerializeReference] public BlackboardVariable<Vector2> Position;

    private MovementComponent _character;
    private AIPath _pathfinder;
    
    protected override Status OnStart()
    {
        GameObject.GetComponentChecked(out _character);
        GameObject.GetComponentChecked(out _pathfinder);
            
        _pathfinder.maxSpeed = _character.MovementSpeed;
        _pathfinder.whenCloseToDestination = CloseToDestinationMode.Stop;
            
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        _pathfinder.destination = Position.Value;
        
        return _pathfinder.remainingDistance <= 0.2f
            ? Status.Success
            : Status.Running;
    }

    protected override void OnEnd()
    {
        _pathfinder.maxSpeed = 0.0f;
    }
}

