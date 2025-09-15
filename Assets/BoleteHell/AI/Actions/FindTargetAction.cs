using System;
using BoleteHell.AI.Boilerplate;
using BoleteHell.AI.Services;
using BoleteHell.Code.Audio.BoleteHell.Models;
using BoleteHell.Gameplay.Character;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Zenject;

namespace BoleteHell.AI.Actions
{
    [Serializable]
    [GeneratePropertyBag]
    [NodeDescription(
        name: "Find suitable target", 
        story: "[Agent] locates target, stores in [Target]", 
        icon: "Assets/Art/Cursor.png",
        category: "Bolete Hell",
        id: "ab42fd85c68c2ece114cb2058a600000")]
    public class FindTargetAction : BoleteAction
    {
        [SerializeReference] 
        public BlackboardVariable<GameObject> Agent;
        
        [SerializeReference]
        public BlackboardVariable<GameObject> Target;
        
        [Inject]
        private IDirector _director;

        protected override Status OnStartImpl()
        {
            var character = Agent.Value.GetComponent<Character>();
            if (!character)
            {
                Debug.LogError("Agent does not have a Character component");
                return Status.Failure;
            }

            ISceneObject target = _director.FindTarget(character);
            if (target is not MonoBehaviour go)
            {
                return Status.Failure;
            }

            Target.Value = go.gameObject;
            return Status.Success;
        }

        protected override Status OnUpdate()
        {
            return Status.Success;
        }

        protected override void OnEnd()
        {
        }
    }
}