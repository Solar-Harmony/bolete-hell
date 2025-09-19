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
    [NodeDescription(name: "Shoot", story: "[Self] shoots at [CurrentTarget]", category: "Bolete Hell", id: "1f4887e471cff4cb12a02b34acc3ea39")]
    public partial class ShootAction : BoleteAction
    {
        [SerializeReference] public BlackboardVariable<GameObject> Self;
        [SerializeReference] public BlackboardVariable<GameObject> CurrentTarget;
        
        [Inject]
        private ITargetingUtils _targeting;

        private Arsenal.Arsenal _arsenal;
    
        
        //TODO: Devrait peut-être avoir une méthode attack dans les ennemis qui est appeler ici plutot que de faire spécifiquement shoot
        //Comme ca ça peut fonctionner pour les ennemis qui attack mélée ou qui explose
        protected override Status OnStartImpl()
        {
            ((IRequestManualInject)this).InjectDependencies();

            if (Self.Value == null || CurrentTarget.Value == null)
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
            
            Vector2 selfPosition = Self.Value.transform.position;
            Vector2 selfVelocity = Self.Value.GetComponent<AIPath>()?.desiredVelocity ?? Vector2.zero;
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