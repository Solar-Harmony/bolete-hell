using System;
using BoleteHell.Code.Audio;
using BoleteHell.Code.Gameplay.Characters;
using BoleteHell.Code.Graphics;
using Sirenix.OdinInspector;
using Zenject;

namespace BoleteHell.Code.Gameplay.Damage.Effects.Impl
{
    [Serializable]
    public sealed class HealStatusEffectConfig : StatusEffectConfig
    {
        [MinValue(1)]
        public int healingEachTick = 10;
    }
    
    public sealed class HealStatusEffect : IStatusEffect<HealStatusEffectConfig>
    {
        public bool CanApply(IStatusEffectTarget target, HealStatusEffectConfig config)
        {
            return target is IDamageable;
        }

        public void Apply(IStatusEffectTarget target, HealStatusEffectConfig config)
        {
            if (target is IDamageable damageable)
            {
                damageable.Health.Heal(config.healingEachTick);
            }
        }

        public void Unapply(IStatusEffectTarget target, HealStatusEffectConfig config)
        {
            if (target is IDamageable damageable)
            {
                damageable.Health.TakeDamage(config.healingEachTick);
            }
        }
    }
}