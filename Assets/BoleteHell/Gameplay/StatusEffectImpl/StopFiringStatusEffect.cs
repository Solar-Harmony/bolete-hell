using System;
using BoleteHell.Code.Arsenal;
using BoleteHell.Code.Gameplay.Damage.Effects;
using BoleteHell.Gameplay.StatusEffectImpl;
using BoleteHell.Utils.Extensions;
using UnityEngine;


[Serializable]
public sealed class StopFiringStatusEffectConfig : StatusEffectConfig
{
    
}

public class StopFiringStatusEffect :IStatusEffect<StopFiringStatusEffectConfig>
{
    public bool CanApply(GameObject target, StopFiringStatusEffectConfig config)
    {
        return target.HasComponent<Arsenal>();
    }

    public void Apply(GameObject target, StopFiringStatusEffectConfig config)
    {
        target.GetComponent<Arsenal>().SetCooldownModifier(9999);
    }

    public void Unapply(GameObject target, StopFiringStatusEffectConfig config)
    {
        target.GetComponent<Arsenal>().SetCooldownModifier(1);

    }
}

