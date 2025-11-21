using System;
using BoleteHell.Code.AI.Services;
using BoleteHell.Code.Core;
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
        [SerializeReference] [CreateProperty] public BlackboardVariable<float> TurnSpeed = new(5f);
        
        private ITargetingUtils _targeting;
        private Arsenal.Arsenal _arsenal;
        private AIPath _pathfinder;
        private Vector2 _currentAimDirection;
        
        protected override Status OnStart()
        {
            if (CurrentTarget.Value == null)
                return Status.Failure;
            ServiceLocator.Get(ref _targeting);
            Debug.Assert(_arsenal ??= Self.Value.GetComponent<Arsenal.Arsenal>());
            _pathfinder ??= Self.Value.GetComponent<AIPath>();
            
            if (_currentAimDirection == Vector2.zero)
            {
                Transform selfTransform = Self.Value.transform;
                _currentAimDirection = selfTransform.right;
            }
            
            Vector2 selfPosition = Self.Value.transform.position;
            Vector2 selfVelocity = _pathfinder?.desiredVelocity ?? Vector2.zero;
            Vector2 targetPosition = CurrentTarget.Value.transform.position;
            Vector2 targetVelocity = CurrentTarget.Value.TryGetComponent(out Rigidbody2D rb)
                ? rb.linearVelocity
                : Vector2.zero;
            float projectileSpeed = _arsenal.GetProjectileSpeed();
            _targeting.SuggestProjectileDirection(out Vector2 targetDirection, projectileSpeed, selfPosition, selfVelocity, targetPosition, targetVelocity);
            
            _currentAimDirection = Vector2.Lerp(_currentAimDirection, targetDirection, TurnSpeed.Value * Time.deltaTime).normalized;
            
            _arsenal.Shoot(_currentAimDirection);
            
            return Status.Success;
        }

        protected override void OnEnd()
        {
        }
    }
}