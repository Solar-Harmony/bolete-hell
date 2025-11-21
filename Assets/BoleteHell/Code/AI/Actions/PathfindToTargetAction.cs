using System;
using BoleteHell.Code.Gameplay.Characters;
using BoleteHell.Code.Utils;
using Pathfinding;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

namespace BoleteHell.Code.AI.Actions
{
    [Serializable]
    [GeneratePropertyBag]
    [NodeDescription(
        name: "Pathfind to target", 
        story: "Pathfind towards [Target]", 
        category: "Bolete Hell",
        id: "ab42fd85c68c2ece114cb2058a607833")]
    public class PathfindToTargetAction : Action
    {
        [SerializeReference] public BlackboardVariable<Transform> Target;

        private Enemy _selfCharacter;
        private AIPath _pathfinder;

        protected override Status OnStart()
        {
            GameObject.GetComponentChecked(out _selfCharacter);
            GameObject.GetComponentChecked(out _pathfinder);
            
            _pathfinder.maxSpeed = _selfCharacter.MovementSpeed;
            _pathfinder.whenCloseToDestination = CloseToDestinationMode.Stop;
            
            return Status.Running;
        }

        protected override Status OnUpdate()
        {
            if (!Target.Value)
            {
                return Status.Failure;
            }
            
            _pathfinder.destination = Target.Value.position;
            
            return _pathfinder.remainingDistance <= 0.2f
                ? Status.Success
                : Status.Running;
        }

        protected override void OnEnd()
        {
            _pathfinder.maxSpeed = 0.0f;
        }
    }
}