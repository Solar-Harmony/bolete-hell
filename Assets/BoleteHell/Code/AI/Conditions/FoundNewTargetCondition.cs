using System;
using BoleteHell.Code.AI.Boilerplate;
using BoleteHell.Code.AI.Services;
using BoleteHell.Code.Gameplay.Character;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Zenject;

namespace BoleteHell.Code.AI.Conditions
{
    [Serializable, GeneratePropertyBag]
    [Condition(name: "Closer target available", story: "[SelfCharacter] found a new [target]", category: "Bolete Hell", id: "a6c458a36beedbba632aa9f9dc2a4d70")]
    public partial class FoundNewTargetCondition : BoleteCondition
    {
        [SerializeReference] public BlackboardVariable<Character> SelfCharacter;
        [SerializeReference] public BlackboardVariable<GameObject> Target;

        [Inject]
        private IDirector _director;
        
        public override bool IsTrueImpl()
        {
            ISceneObject target = _director.FindTarget(SelfCharacter);
            if (target is not MonoBehaviour go)
            {
                Debug.LogError($"{SelfCharacter.Name} targeted a non MonoBehaviour {target}");
                return false;
            }

            if (Target.Value == go.gameObject) 
                return false;

            Target.Value = go.gameObject;
            return true;
        }

        public override void OnStart()
        {
        }

        public override void OnEnd()
        {
        }
    }
}
