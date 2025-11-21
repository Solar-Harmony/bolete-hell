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
        name: "Find unhealthiest ally", 
        story: "Find unhealthiest ally, store in [CurrentTarget]", 
        category: "Bolete Hell", id: "ab42fd85c68c2ece114cb2058a600001")]
    public class FindWeakestAlly : Action
    {
        [SerializeReference]
        public BlackboardVariable<GameObject> CurrentTarget;
        
        private IDirector _director;
        private Character _character;

        protected override Status OnStart()
        {
            ServiceLocator.Get(out _director);
            
            GameObject.GetComponentChecked(out _character);
            
            return Status.Running;
        }

        protected override Status OnUpdate()
        {
            ISceneObject target = _director.FindNearestAlly(_character);
            
            CurrentTarget.Value = target is Enemy npc && npc.Health.Percent < 1.0f
                ? npc.gameObject
                : null;

            return Status.Success;
        }
    }
}