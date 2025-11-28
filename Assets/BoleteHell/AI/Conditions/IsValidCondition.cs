using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;

namespace BoleteHell.AI.Conditions
{
    public enum ValidityOption { Is, IsNot }

    [Serializable, GeneratePropertyBag]
    [Condition(name: "Is Valid", story: "[Object] [Option] valid", category: "Bolete Hell", id: "db7df5087732cb6e9ec786290eb618cf")]
    public partial class IsValidCondition : Condition
    {
        [SerializeReference] public BlackboardVariable<GameObject> Object;
        [SerializeReference] public BlackboardVariable<ValidityOption> Option;
        
        public override bool IsTrue()
        {
            if (Option == ValidityOption.Is)
                return Object.Value;
            
            return !Object.Value;
        }
    }
}
