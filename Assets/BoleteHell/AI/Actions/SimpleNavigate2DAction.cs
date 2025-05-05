using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

namespace AI.Actions
{
    [Serializable]
    [GeneratePropertyBag]
    [NodeDescription("Simple Navigate 2D ", story: "[Agent] navigatesto [Target]", category: "Action",
        id: "ab42fd85c68c2ece114cb2058a607833")]
    public class SimpleNavigate2DAction : Action
    {
        [SerializeReference] public BlackboardVariable<GameObject> Agent;
        [SerializeReference] public BlackboardVariable<GameObject> Target;
        [SerializeReference] public BlackboardVariable<float> Speed;

        protected override Status OnStart()
        {
            return Status.Running;
        }

        protected override Status OnUpdate()
        {
            if (Agent.Value == null || Target.Value == null) return Status.Failure;

            // Get the agent's transform
            var agentTransform = Agent.Value.transform;
            // Get the target's transform

            var targetTransform = Target.Value.transform;

            // Calculate the direction to the target
            var direction = (targetTransform.position - agentTransform.position).normalized;
            // Calculate the distance to the target
            var distance = Vector3.Distance(agentTransform.position, targetTransform.position);

            // Move the agent towards the target
            agentTransform.position += direction * Speed * Time.deltaTime;
            // Check if the agent has reached the target

            if (distance < 5.0f)
                // Stop the agent
                // agentTransform.position = targetTransform.position;
                return Status.Success;

            // Continue moving towards the target
            return Status.Running;
        }

        protected override void OnEnd()
        {
        }
    }
}