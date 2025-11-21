using System;
using BoleteHell.Code.AI.Services;
using BoleteHell.Code.Core;
using BoleteHell.Code.Gameplay.Characters;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;

namespace BoleteHell.Code.AI.Conditions
{
    [Serializable, GeneratePropertyBag]
    [Condition(name: "Closer enemy available", story: "[SelfCharacter] found a new [target]", category: "Bolete Hell", id: "a6c458a36beedbba632aa9f9dc2a4d71")]
    public partial class FoundNewEnemyCondition : Condition
    {
        [SerializeReference] public BlackboardVariable<Character> SelfCharacter;
        [SerializeReference] public BlackboardVariable<GameObject> Target;

        private IDirector _director;
        
        public override bool IsTrue()
        {
            ServiceLocator.Get(ref _director);
            
            ISceneObject target = _director.FindClosestShroom(SelfCharacter);
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
