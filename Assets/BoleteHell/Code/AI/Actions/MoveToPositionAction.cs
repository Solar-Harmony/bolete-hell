using System;
using BoleteHell.Code.Gameplay.Characters;
using Pathfinding;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "MoveToPosition", story: "[Self] pathfinds towards [position]", category: "Bolete Hell", id: "4d6c55f8d197383e9b5f4ea442dce9bb")]
public partial class MoveToPositionAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<Vector2> Position;

    private Enemy _selfCharacter;
    private AIPath _pathfinder;
    protected override Status OnStart()
    {
        Debug.Assert(_pathfinder ??= Self.Value.GetComponent<AIPath>());
        Debug.Assert(_selfCharacter ??= Self.Value.GetComponent<Enemy>());
            
        _pathfinder.maxSpeed = _selfCharacter.MovementSpeed;
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

