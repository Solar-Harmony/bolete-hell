using System;
using Pathfinding;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using UnityEngine.Serialization;
using Action = Unity.Behavior.Action;

namespace AI.Actions
{
    [Serializable]
    [GeneratePropertyBag]
    [NodeDescription(
        name: "Navigate in range (2D)", 
        story: "[Agent] navigates in range of [Target]", 
        description: "Use 2D pathfinding to navigate towards the target until within a certain range.",
        icon: "Assets/Art/Cursor.png",
        category: "Bolete Hell",
        id: "ab42fd85c68c2ece114cb2058a607833")]
    public class Navigate2DAction : Action
    {
        [SerializeReference] public BlackboardVariable<GameObject> Agent;
        [SerializeReference] public BlackboardVariable<GameObject> Target;
        [SerializeReference] public BlackboardVariable<float> Range;
        [SerializeReference] public BlackboardVariable<float> MaxSpeed;
        
        private AIPath _pathfinder;

        protected override Status OnStart()
        {
            return Status.Running;
        }

        protected override Status OnUpdate()
        {
            if (Agent.Value == null || Target.Value == null) 
                return Status.Failure;
            
            if (_pathfinder == null)
            {
                _pathfinder = Agent.Value.GetComponent<AIPath>();
                if (_pathfinder == null)
                {
                    Debug.LogError("AIPath component not found on the agent.");
                    return Status.Failure;
                }
            }
            
            _pathfinder.maxSpeed = MaxSpeed.Value;
            _pathfinder.endReachedDistance = Range;
            _pathfinder.destination = Target.Value.transform.position;
            _pathfinder.whenCloseToDestination = CloseToDestinationMode.Stop;
            
            return _pathfinder.reachedDestination ? Status.Success : Status.Running;
        }

        protected override void OnEnd()
        {
        }
    }
}