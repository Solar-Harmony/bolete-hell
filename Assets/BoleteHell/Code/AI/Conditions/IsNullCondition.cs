using System;
using BoleteHell.Code.AI.Boilerplate;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;

namespace BoleteHell.Code.AI.Conditions
{
    [Serializable, GeneratePropertyBag]
    [Condition(name: "Is Null", story: "[Object] is Null", category: "Bolete Hell", id: "db7df5087732cb6e9ec786290eb618cf")]
    public partial class IsNullCondition : BoleteCondition
    {
        [SerializeReference] public BlackboardVariable<GameObject> Object;
        
        public override bool IsTrueImpl()
        {
            return !Object.Value;
        }

        public override void OnStart()
        {
        }

        public override void OnEnd()
        {
        }
    }
}
