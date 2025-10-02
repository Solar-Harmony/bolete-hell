using System;
using BoleteHell.Code.AI.Boilerplate;
using BoleteHell.Code.AI.Services;
using BoleteHell.Code.Gameplay.Character;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Zenject;

namespace BoleteHell.Code.AI.Actions
{
    [Serializable]
    [GeneratePropertyBag]
    [NodeDescription(
        name: "Find suitable target", 
        story: "[Agent] finds nearest target, stores in [Target]", 
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

        private Character character;

        protected override Status OnStartImpl()
        {
            character = Agent.Value.GetComponent<Character>();
            if (!character)
            {
                Debug.LogError("Agent does not have a Character component");
                return Status.Failure;
            }
            return Status.Running;
        }

        protected override Status OnUpdate()
        {
            ISceneObject target = _director.FindTarget(character);
            if (target is not MonoBehaviour go)
            {
                Debug.LogError($"{Agent.Name} targeted a non MonoBehaviour {target}");
                return Status.Failure;
            }

            Target.Value = go.gameObject;
            return Status.Success;
        }

        protected override void OnEnd()
        {
        }
    }
}