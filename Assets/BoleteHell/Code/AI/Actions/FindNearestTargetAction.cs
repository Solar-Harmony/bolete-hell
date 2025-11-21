using System;
using BoleteHell.Code.AI.Services;
using BoleteHell.Code.Core;
using BoleteHell.Code.Gameplay.Characters;
using BoleteHell.Code.Utils;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

namespace BoleteHell.Code.AI.Actions
{
    [Serializable]
    [GeneratePropertyBag]
    [NodeDescription(
        name: "Find nearest target", 
        story: "Find nearest target, store in [Target]", 
        category: "Bolete Hell", id: "ab42fd85c68c2ece114cb2058a600000")]
    public class FindNearestTargetAction : Action
    {
        [SerializeReference]
        public BlackboardVariable<GameObject> Target;
        
        private IDirector _director;
        private Character character;

        protected override Status OnStart()
        {
            ServiceLocator.Get(out _director);

            GameObject.GetComponentChecked(out character);
            
            return Status.Running;
        }

        protected override Status OnUpdate()
        {
            ISceneObject target = _director.FindNearestTarget(character);
            if (target is not MonoBehaviour go)
            {
                Debug.LogError($"{GameObject.name} targeted a non MonoBehaviour {target}");
                return Status.Failure;
            }

            Target.Value = go.gameObject;
            return Status.Success;
        }
    }
}