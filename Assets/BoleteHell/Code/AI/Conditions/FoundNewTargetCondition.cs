using System;
using BoleteHell.Code.AI.Services;
using BoleteHell.Code.Core;
using BoleteHell.Code.Gameplay.Characters;
using BoleteHell.Code.Utils;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;

namespace BoleteHell.Code.AI.Conditions
{
    [Serializable, GeneratePropertyBag]
    [Condition(
        name: "Closer target available", 
        story: "Closer target is available ➔ [Target]", 
        category: "Bolete Hell", id: "a6c458a36beedbba632aa9f9dc2a4d70")]
    public partial class FoundNewTargetCondition : Condition
    {
        [SerializeReference] public BlackboardVariable<GameObject> Target;

        private IDirector _director;
        private Character _character;

        public override void OnStart()
        {
            ServiceLocator.Get(out _director);
            GameObject.GetComponentChecked(out _character);
        }

        public override bool IsTrue()
        {
            ISceneObject target = _director.FindNearestTarget(_character);
            if (target is not MonoBehaviour go)
            {
                Debug.LogError($"{_character.name} targeted a non MonoBehaviour {target}");
                return false;
            }

            if (Target.Value == go.gameObject) 
                return false;

            Target.Value = go.gameObject;
            return true;
        }
    }
}
