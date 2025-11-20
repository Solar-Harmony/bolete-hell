using System;
using BoleteHell.Code.AI.Services;
using BoleteHell.Code.Core;
using BoleteHell.Code.Gameplay.Characters;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Zenject;
using Action = Unity.Behavior.Action;

namespace BoleteHell.Code.AI.Actions
{
    //On pourras faire un action qui permet de
    //changer quel valeur qu'on cherche nearest, farthest, weakest wtv
    //et pouvoir spécifier si on cherche un allié un ennemi
    [Serializable]
    [GeneratePropertyBag]
    [NodeDescription(
        name: "Find closest enemy", 
        story: "[Self] finds nearest enemy, stores in [CurrentTarget]", 
        icon: "Assets/Art/Cursor.png",
        category: "Bolete Hell",
        id: "ab42fd85c68c2ece114cb2058a600001")]
    public class FindClosestEnemy : Action
    {
        [SerializeReference] 
        public BlackboardVariable<GameObject> Self;
        
        [SerializeReference]
        public BlackboardVariable<GameObject> CurrentTarget;
        
        private IDirector _director;

        private Character character;

        protected override Status OnStart()
        {
            ServiceLocator.Get(ref _director);
            character = Self.Value.GetComponent<Character>();
            if (!character)
            {
                Debug.LogError("Agent does not have a Character component");
                return Status.Failure;
            }
            
            return Status.Running;
        }

        protected override Status OnUpdate()
        {
            ServiceLocator.Get(ref _director);
            MonoBehaviour target = _director.FindClosestShroom(character) as MonoBehaviour;

            CurrentTarget.Value = target == null ? null : target.gameObject;
            return Status.Success;
        }

        protected override void OnEnd()
        {
        }
    }
}