using System;
using BoleteHell.AI.Boilerplate;
using BoleteHell.AI.Services;
using BoleteHell.Arsenals;
using BoleteHell.Utils;
using Pathfinding;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Zenject;

namespace BoleteHell.AI.Actions
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "Shoot", story: "[Self] shoots at [Target]", category: "Bolete Hell", id: "1f4887e471cff4cb12a02b34acc3ea39")]
    public partial class ShootAction : BoleteAction
    {
        [SerializeReference] public BlackboardVariable<GameObject> Self;
        [SerializeReference] public BlackboardVariable<GameObject> Target;
        
        [Inject]
        private ITargetingUtils _targeting;
    
        protected override Status OnStartImpl()
        {
            ServiceLocator.Inject(this);

            if (Self.Value == null || Target.Value == null)
            {
                Debug.LogError("Self or Target is null");
                return Status.Failure;
            }
        
            // TODO: make arsenal config only and have shooting service
            if (!Self.Value.TryGetComponent(out Arsenal arsenal))
            {
                Debug.LogError("Self has no arsenal");
                return Status.Failure;
            }
            
            Vector2 selfPosition = Self.Value.transform.position;
            Vector2 selfVelocity = Self.Value.GetComponent<AIPath>()?.desiredVelocity ?? Vector2.zero;
            Vector2 targetPosition = Target.Value.transform.position;
            Vector2 targetVelocity = Target.Value.TryGetComponent(out Rigidbody2D rb)
                ? rb.linearVelocity
                : Vector2.zero;
            float projectileSpeed = arsenal.GetProjectileSpeed();
            _targeting.SuggestProjectileDirection(out Vector2 direction, projectileSpeed, selfPosition, selfVelocity, targetPosition, targetVelocity);
            arsenal.Shoot(direction);
        
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
}