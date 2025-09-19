using System;
using BoleteHell.Code.AI.Boilerplate;
using BoleteHell.Code.AI.Services;
using BoleteHell.Code.Gameplay.Character;
using Pathfinding;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using UnityEngine.Rendering;
using Zenject;

namespace BoleteHell.Code.AI.Actions
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
    public class Navigate2DAction : BoleteAction
    {
        [SerializeReference] public BlackboardVariable<GameObject> Agent;
        [SerializeReference] public BlackboardVariable<GameObject> Target;
        [SerializeReference] public BlackboardVariable<Enemy> character;
        [SerializeReference] public BlackboardVariable<float> Range;
        
        private AIPath _pathfinder;
        
        [Inject]
        private ITargetingUtils _targeting;

        protected override Status OnStartImpl()
        {
            return Status.Running;
        }

        protected override Status OnUpdate()
        {
            GameObject agent = Agent.Value;
            GameObject target = Target.Value;
            
            if (!agent || !target) 
                return Status.Failure;
            
            if (!(_pathfinder ??= Agent.Value.GetComponent<AIPath>()))
            {
                Debug.LogError("AIPath component not found on the agent.");
                return Status.Failure;
            }

            _pathfinder.maxSpeed = character.Value.MovementSpeed;
            _pathfinder.endReachedDistance = Range;
            _pathfinder.destination = Target.Value.transform.position;
            _pathfinder.whenCloseToDestination = CloseToDestinationMode.Stop;
            
            bool bHasLineOfSight = _targeting.HasLineOfSight(agent, target, 1000); // TODO: use range value in agent
            if (bHasLineOfSight)
            {
                _pathfinder.maxSpeed = 0.0f; // stop moving if we have line of sight
            }
            return _pathfinder.reachedDestination || bHasLineOfSight ? Status.Success : Status.Running;
        }

        protected override void OnEnd()
        {
        }
    }
}