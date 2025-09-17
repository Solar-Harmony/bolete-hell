using System;
using BoleteHell.Code.AI.Boilerplate;
using BoleteHell.Code.AI.Services;
using Pathfinding;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Zenject;

namespace BoleteHell.Code.AI.Actions
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "Shoot", story: "[Self] shoots at [Target]", category: "Bolete Hell", id: "1f4887e471cff4cb12a02b34acc3ea39")]
    public partial class ShootAction : BoleteAction
    {
        [SerializeReference] public BlackboardVariable<GameObject> Self;
        [SerializeReference] public BlackboardVariable<GameObject> Target;
        
        [Inject]
        private ITargetingUtils _targeting;

        private Arsenal.Arsenal _arsenal;
    
        
        //TODO: Devrait peut-être avoir une méthode attack dans les ennemis qui est appeler ici plutot que de faire spécifiquement shoot
        //Comme ca ça peut fonctionner pour les ennemis qui attack mélée ou qui explose
        protected override Status OnStartImpl()
        {
            ((IRequestManualInject)this).InjectDependencies();

            if (Self.Value == null || Target.Value == null)
            {
                Debug.LogError("Self or Target is null");
                return Status.Failure;
            }
        
            // TODO: make arsenal config only and have shooting service
            if (!Self.Value.TryGetComponent(out _arsenal))
            {
                Debug.LogError("Self has no arsenal");
                return Status.Failure;
            }
            
            
            return Status.Running;
        }

        protected override Status OnUpdate()
        {
            Vector2 selfPosition = Self.Value.transform.position;
            Vector2 selfVelocity = Self.Value.GetComponent<AIPath>()?.desiredVelocity ?? Vector2.zero;
            Vector2 targetPosition = Target.Value.transform.position;
            Vector2 targetVelocity = Target.Value.TryGetComponent(out Rigidbody2D rb)
                ? rb.linearVelocity
                : Vector2.zero;
            float projectileSpeed = _arsenal.GetProjectileSpeed();
            _targeting.SuggestProjectileDirection(out Vector2 direction, projectileSpeed, selfPosition, selfVelocity, targetPosition, targetVelocity);
            _arsenal.Shoot(direction);
            return Status.Running;
        }

        protected override void OnEnd()
        {
        }
    }
}