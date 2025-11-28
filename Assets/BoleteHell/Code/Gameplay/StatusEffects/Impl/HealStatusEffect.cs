using System;
using BoleteHell.Utils.Extensions;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BoleteHell.Code.Gameplay.Damage.Effects.Impl
{
    using IStatusEffectTarget = GameObject;
    [Serializable]
    public sealed class HealStatusEffectConfig : StatusEffectConfig
    {
        [MinValue(1)]
        public int HealingEachTick = 10;
    }
    
    public sealed class HealStatusEffect : IStatusEffect<HealStatusEffectConfig>
    {
        public bool CanApply(IStatusEffectTarget target, HealStatusEffectConfig config)
        {
            return target.HasComponent<HealthComponent>();
        }

        public void Apply(IStatusEffectTarget target, HealStatusEffectConfig config)
        {
            target.GetComponent<HealthComponent>().Heal(config.HealingEachTick);
        }

        public void Unapply(IStatusEffectTarget target, HealStatusEffectConfig config)
        {
            target.GetComponent<HealthComponent>().TakeDamage(config.HealingEachTick);
        }
    }
}