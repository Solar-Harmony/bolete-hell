using System;
using BoleteHell.Gameplay.Characters;
using BoleteHell.Utils.Extensions;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;

[Serializable, GeneratePropertyBag]
[Condition(
    name: "Compare health",
    story: "[Entity] health is [X] [N] %", 
    category: "Bolete Hell", id: "6e09d02aa747cec522698c86945beba2")]
public partial class CompareHealthCondition : Condition
{
    [SerializeReference] public BlackboardVariable<GameObject> Entity;
    [Comparison(comparisonType: ComparisonType.All)]
    [SerializeReference] public BlackboardVariable<ConditionOperator> X;
    [SerializeReference] public BlackboardVariable<int> N;

    public override bool IsTrue()
    {
        Entity.Value.GetComponentChecked(out HealthComponent healthComponent);
        float health = healthComponent.Percent;
        float ratio = N.Value / 100.0f;
        
        return X.Value switch
        {
            ConditionOperator.Lower => health < ratio,
            ConditionOperator.LowerOrEqual => health <= ratio,
            ConditionOperator.Equal => Mathf.Approximately(health, ratio),
            ConditionOperator.GreaterOrEqual => health >= ratio,
            ConditionOperator.Greater => health > ratio,
            ConditionOperator.NotEqual => !Mathf.Approximately(health, ratio),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}
