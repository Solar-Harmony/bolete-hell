using System;
using BoleteHell.Code.Gameplay.Damage;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;

[Serializable, GeneratePropertyBag]
[Condition(name: "Compare health", story: "[Self] health is [X] than [N]", category: "Bolete Hell", id: "6e09d02aa747cec522698c86945beba2")]
public partial class CompareHealthCondition : Condition
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [Comparison(comparisonType: ComparisonType.All)]
    [SerializeReference] public BlackboardVariable<ConditionOperator> X;
    [SerializeReference] public BlackboardVariable<int> N;

    public override bool IsTrue()
    {
        var health = Self.Value.GetComponent<Health>().CurrentHealth;
        return X.Value switch
        {
            ConditionOperator.Lower => health < N.Value,
            ConditionOperator.LowerOrEqual => health <= N.Value,
            ConditionOperator.Equal => health == N.Value,
            ConditionOperator.GreaterOrEqual => health >= N.Value,
            ConditionOperator.Greater => health > N.Value,
            ConditionOperator.NotEqual => health != N.Value,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
