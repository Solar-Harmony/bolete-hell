using System;
using BoleteHell.Code.Gameplay.Characters;
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
        name: "Navigate in range (2D)", 
        story: "[Self] pathfinds towards [CurrentTarget]", 
        description: "Use 2D pathfinding to navigate towards the target until within a certain range.",
        icon: "Assets/Art/Cursor.png",
        category: "Bolete Hell",
        id: "ab42fd85c68c2ece114cb2058a607833")]
    public class Navigate2DAction : Action
    {
        [SerializeReference] public BlackboardVariable<GameObject> Self;
        [SerializeReference] public BlackboardVariable<GameObject> CurrentTarget;

        private Enemy _selfCharacter;
        private AIPath _pathfinder;

        protected override Status OnStart()
        {
            Debug.Assert(_pathfinder ??= Self.Value.GetComponent<AIPath>());
            Debug.Assert(_selfCharacter ??= Self.Value.GetComponent<Enemy>());
            
            _pathfinder.maxSpeed = _selfCharacter.MovementSpeed;
            _pathfinder.whenCloseToDestination = CloseToDestinationMode.Stop;
            
            return Status.Running;
        }

        protected override Status OnUpdate()
        {

            if (CurrentTarget.Value == null)
            {
                return Status.Failure;
            }
            
            _pathfinder.destination = CurrentTarget.Value.transform.position;


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