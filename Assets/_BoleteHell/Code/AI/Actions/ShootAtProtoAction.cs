using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Object = UnityEngine.Object;

namespace AI.Actions
{
    [Serializable]
    [GeneratePropertyBag]
    [NodeDescription("Shoot at (prototype)", story: "[Self] shoots [ProjectilePrefab] at [Target]", category: "Bolete Hell",
        id: "595d95f6a9ce37b61afe9df7df770a05")]
    public class ShootAtProtoAction : Action
    {
        [SerializeReference] public BlackboardVariable<GameObject> Self;
        [SerializeReference] public BlackboardVariable<GameObject> ProjectilePrefab;
        [SerializeReference] public BlackboardVariable<GameObject> Target;

        protected override Status OnStart()
        {
            return Status.Running;
        }

        protected override Status OnUpdate()
        {
            if (Self.Value == null || Target.Value == null || ProjectilePrefab.Value == null) return Status.Failure;

            // Get the attack's transform
            var attackTransform = Self.Value.transform;
            // Get the player's transform
            var playerTransform = Target.Value.transform;

            // Calculate the direction to the player
            var direction = (playerTransform.position - attackTransform.position).normalized;
            // Calculate the distance to the player
            var distance = Vector3.Distance(attackTransform.position, playerTransform.position);

            var projectile = Object.Instantiate(ProjectilePrefab.Value, attackTransform.position, Quaternion.identity);
            var rb = projectile.GetComponent<Rigidbody2D>();
            Physics2D.IgnoreCollision(
                projectile.GetComponent<Collider2D>(),
                Self.Value.GetComponent<Collider2D>()
            );
            if (rb != null)
                rb.linearVelocity = direction * 10f; // Set the projectile speed
            else
                Debug.LogError("Rigidbody2D component not found on the projectile prefab.");

            return Status.Success;
        }

        protected override void OnEnd()
        {
        }
    }
}