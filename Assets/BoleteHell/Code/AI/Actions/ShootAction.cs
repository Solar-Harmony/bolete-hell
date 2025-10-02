using System;
using BoleteHell.Code.AI.Services;
using Pathfinding;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

namespace BoleteHell.Code.AI.Actions
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "Shoot", story: "[Self] shoots at [CurrentTarget]", category: "Bolete Hell", id: "1f4887e471cff4cb12a02b34acc3ea39")]
    public partial class ShootAction : Action
    {
        [SerializeReference] public BlackboardVariable<GameObject> Self;
        [SerializeReference] public BlackboardVariable<GameObject> CurrentTarget;
        
        private ITargetingUtils _targeting;
        private Arsenal.Arsenal _arsenal;
        private AIPath _pathfinder;
        
        protected override Status OnStart()
        {
            ServiceLocator.Get(ref _targeting);
            Debug.Assert(_arsenal ??= Self.Value.GetComponent<Arsenal.Arsenal>());
            _pathfinder ??= Self.Value.GetComponent<AIPath>();
            
            Vector2 selfPosition = Self.Value.transform.position;
            Vector2 selfVelocity = _pathfinder?.desiredVelocity ?? Vector2.zero;
            Vector2 targetPosition = CurrentTarget.Value.transform.position;
            Vector2 targetVelocity = CurrentTarget.Value.TryGetComponent(out Rigidbody2D rb)
                ? rb.linearVelocity
                : Vector2.zero;
            float projectileSpeed = _arsenal.GetProjectileSpeed();
            _targeting.SuggestProjectileDirection(out Vector2 direction, projectileSpeed, selfPosition, selfVelocity, targetPosition, targetVelocity);
            _arsenal.Shoot(direction);
            
            return Status.Success;
        }

        protected override void OnEnd()
        {
        }
    }
}