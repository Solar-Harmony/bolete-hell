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
        story: "[Self] navigates in range of [CurrentTarget]", 
        description: "Use 2D pathfinding to navigate towards the target until within a certain range.",
        icon: "Assets/Art/Cursor.png",
        category: "Bolete Hell",
        id: "ab42fd85c68c2ece114cb2058a607833")]
    public class Navigate2DAction : BoleteAction
    {
        [SerializeReference] public BlackboardVariable<GameObject> Self;
        [SerializeReference] public BlackboardVariable<GameObject> CurrentTarget;
        [SerializeReference] public BlackboardVariable<Enemy> Character;
        
        private AIPath _pathfinder;
        
        [Inject]
        private ITargetingUtils _targeting;
        
        

        protected override Status OnStartImpl()
        {
            return Status.Running;
        }

        protected override Status OnUpdate()
        {
            GameObject agent = Self.Value;
            GameObject target = CurrentTarget.Value;
            
            if (!agent || !target) 
                return Status.Failure;
            
            if (!(_pathfinder ??= Self.Value.GetComponent<AIPath>()))
            {
                Debug.LogError("AIPath component not found on the agent.");
                return Status.Failure;
            }

            _pathfinder.maxSpeed = Character.Value.MovementSpeed;
            _pathfinder.destination = CurrentTarget.Value.transform.position;
            _pathfinder.whenCloseToDestination = CloseToDestinationMode.Stop;
            
            return Status.Running;
        }

        protected override void OnEnd()
        {
            _pathfinder.maxSpeed = 0.0f;
        }
    }
}