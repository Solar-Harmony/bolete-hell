using System;
using BoleteHell.Code.Gameplay.Damage.Effects;
using UnityEngine;

namespace BoleteHell.Gameplay.StatusEffectImpl
{
    [Serializable]
    public sealed class ShrinkStatusEffectConfig : StatusEffectConfig
    {
        public float ShrinkPercent = 0.5f;
    }
    
    public sealed class ShrinkStatusEffect : IStatusEffect<ShrinkStatusEffectConfig>
    {
        public bool CanApply(GameObject target, ShrinkStatusEffectConfig config)
        {
            return true;
        }

        public void Apply(GameObject target, ShrinkStatusEffectConfig config)
        {
            target.transform.localScale *= (1 - config.ShrinkPercent);
        }

        public void Unapply(GameObject target, ShrinkStatusEffectConfig config)
        {
            target.transform.localScale /= (1 - config.ShrinkPercent);
        }
    }
}