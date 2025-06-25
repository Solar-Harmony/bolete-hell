using System;
using _BoleteHell.Code.AI;
using AI.Agents;
using BoleteHell.RayCannon;
using Pathfinding;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Shoot", story: "[Self] shoots at [Target]", category: "Bolete Hell", id: "1f4887e471cff4cb12a02b34acc3ea39")]
public partial class ShootAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    
    protected override Status OnStart()
    {
        if (Self.Value == null || Target.Value == null)
        {
            Debug.LogError("Self or Target is null");
            return Status.Failure;
        }
        
        if (!Self.Value.TryGetComponent(out Enemy enemy))
        {
            Debug.LogError("Self is not an enemy");
            return Status.Failure;
        }
        
        Vector2 selfPosition = Self.Value.transform.position;
        Vector2 selfVelocity = Self.Value.GetComponent<AIPath>().desiredVelocity;
        Vector2 targetPosition = Target.Value.transform.position;
        Vector2 targetVelocity = Target.Value.GetComponent<Rigidbody2D>().linearVelocity;
        float projectileSpeed = enemy.GetProjectileSpeed();
        AIUtils.SuggestProjectileDirection(out Vector2 direction, projectileSpeed, selfPosition, selfVelocity, targetPosition, targetVelocity);
        enemy.Shoot(direction);
        
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}