using System;
using BoleteHell.Code.Arsenal;
using BoleteHell.Utils.Extensions;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

namespace BoleteHell.AI.Actions
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(
        name: "Change weapon", 
        story: "Switch to weapon at index [Index]", 
        category: "Bolete Hell", id: "9467d778927ea8f1fc1502cb826e23d7")]
    public partial class SetSelectedWeaponAction : Action
    {
        [SerializeReference] public BlackboardVariable<int> Index;

        private Arsenal _arsenal;

        protected override Status OnStart()
        {
            GameObject.GetComponentChecked(out _arsenal);
        
            _arsenal.SetSelectedWeaponIndex(Index.Value);
        
            return Status.Success;
        }
    }
}

