using System;
using BoleteHell.Code.AI.Boilerplate;
using BoleteHell.Code.AI.Services;
using BoleteHell.Code.Gameplay.Character;
using Pathfinding;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Zenject;

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
    public class Navigate2DAction : BoleteAction
    {
        [SerializeReference] public BlackboardVariable<GameObject> Self;
        [SerializeReference] public BlackboardVariable<GameObject> CurrentTarget;
        
        [Inject]
        private ITargetingUtils _targeting;
        
        private Enemy _selfCharacter;
        private AIPath _pathfinder;

        protected override Status OnStartImpl()
        {
            if (!Self.Value || !CurrentTarget.Value) 
                return Status.Failure;
            
            Debug.Assert(_pathfinder ??= Self.Value.GetComponent<AIPath>());
            Debug.Assert(_selfCharacter ??= Self.Value.GetComponent<Enemy>());
            
            _pathfinder.maxSpeed = _selfCharacter.MovementSpeed;
            _pathfinder.destination = CurrentTarget.Value.transform.position;
            _pathfinder.whenCloseToDestination = CloseToDestinationMode.Stop;
            
            return Status.Running;
        }

        protected override Status OnUpdate()
        {
            return _pathfinder.remainingDistance <= 0.5
                ? Status.Success
                : Status.Running;
        }

        protected override void OnEnd()
        {
            _pathfinder.maxSpeed = 0.0f;
        }
    }
}