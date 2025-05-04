using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Simple Navigate 2D ", story: "[Agent] navigatesto [Target]", category: "Action", id: "ab42fd85c68c2ece114cb2058a607833")]
public partial class SimpleNavigate2DAction : Action
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
        if (Agent.Value == null || Target.Value == null)
        {
            return Status.Failure;
        }
        
        // Get the agent's transform
        Transform agentTransform = Agent.Value.transform;
        // Get the target's transform
        
        Transform targetTransform = Target.Value.transform;
        
        // Calculate the direction to the target
        Vector3 direction = (targetTransform.position - agentTransform.position).normalized;
        // Calculate the distance to the target
        float distance = Vector3.Distance(agentTransform.position, targetTransform.position);
        
        // Move the agent towards the target
        agentTransform.position += direction * Speed * Time.deltaTime;
        // Check if the agent has reached the target
        
        if (distance < 5.0f)
        {
            // Stop the agent
            // agentTransform.position = targetTransform.position;
            return Status.Success;
        }
        
        // Continue moving towards the target
        return Status.Running;
    }

    protected override void OnEnd()
    {
    }
}

