using System;
using BoleteHell.AI.Services;
using BoleteHell.Code.Arsenal;
using BoleteHell.Code.Core;
using BoleteHell.Utils.Extensions;
using Pathfinding;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

namespace BoleteHell.AI.Actions
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(
        name: "Shoot", 
        story: "Shoot at [CurrentTarget]", 
        category: "Bolete Hell", id: "1f4887e471cff4cb12a02b34acc3ea39")]
    public partial class ShootAction : Action
    {
        [SerializeReference] public BlackboardVariable<GameObject> CurrentTarget;
        [SerializeReference] [CreateProperty] public BlackboardVariable<float> TurnSpeed = new(5f);
        
        private ITargetingUtils _targeting;
        private Arsenal _arsenal;
        private AIPath _pathfinder;
        private Vector2 _currentAimDirection;
        
        protected override Status OnStart()
        {
            if (!CurrentTarget.Value)
                return Status.Failure;
            
            ServiceLocator.Get(out _targeting);
            
            GameObject.GetComponentChecked(out _arsenal);
            GameObject.TryGetComponent(out _pathfinder);
            
            if (_currentAimDirection == Vector2.zero)
            {
                Transform selfTransform = GameObject.transform;
                _currentAimDirection = selfTransform.right;
            }
            
            
            return Status.Running;
        }

        protected override Status OnUpdate()
        {
            if (!CurrentTarget.Value)
                return Status.Failure;
            
            if (_arsenal.IsReadyToShoot())
            {
                Vector2 selfPosition = GameObject.transform.position;
                Vector2 selfVelocity = _pathfinder?.desiredVelocity ?? Vector2.zero;
                Vector2 targetPosition = CurrentTarget.Value.transform.position;
                Vector2 targetVelocity = CurrentTarget.Value.TryGetComponent(out Rigidbody2D rb)
                    ? rb.linearVelocity
                    : Vector2.zero;
                float projectileSpeed = _arsenal.GetProjectileSpeed();
                _targeting.SuggestProjectileDirection(out Vector2 targetDirection, projectileSpeed, selfPosition, selfVelocity, targetPosition, targetVelocity);
                _currentAimDirection = Vector2.Lerp(_currentAimDirection, targetDirection, TurnSpeed.Value * Time.deltaTime).normalized;
                return !_arsenal.Shoot(_currentAimDirection) ? Status.Running : Status.Success;
            }

            // simplify so we don't calculate the whole thing at once
            Vector2 simpleTargetDirection = (CurrentTarget.Value.transform.position - GameObject.transform.position).normalized;
            _currentAimDirection = Vector2.Lerp(_currentAimDirection, simpleTargetDirection, TurnSpeed.Value * Time.deltaTime).normalized;
            return Status.Running;
        }

         protected override void OnEnd()
         {
             _arsenal?.OnShootCanceled();
         }
    }
}