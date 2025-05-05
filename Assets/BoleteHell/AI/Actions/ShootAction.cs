using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

namespace AI.Actions
{
    [Serializable]
    [GeneratePropertyBag]
    [NodeDescription("Shoot", story: "[Attack] at [Player]", category: "Action",
        id: "595d95f6a9ce37b61afe9df7df770a05")]
    public class ShootAction : Action
    {
        [SerializeReference] public BlackboardVariable<GameObject> Attack;
        [SerializeReference] public BlackboardVariable<GameObject> Player;
        [SerializeReference] public BlackboardVariable<GameObject> ProjectilePrefab;

        protected override Status OnStart()
        {
            return Status.Running;
        }

        protected override Status OnUpdate()
        {
            if (Attack.Value == null || Player.Value == null || ProjectilePrefab.Value == null) return Status.Failure;

            // Get the attack's transform
            var attackTransform = Attack.Value.transform;
            // Get the player's transform
            var playerTransform = Player.Value.transform;

            // Calculate the direction to the player
            var direction = (playerTransform.position - attackTransform.position).normalized;
            // Calculate the distance to the player
            var distance = Vector3.Distance(attackTransform.position, playerTransform.position);

            var projectile =
                GameObject.Instantiate(ProjectilePrefab.Value, attackTransform.position, Quaternion.identity);
            var rb = projectile.GetComponent<Rigidbody2D>();
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