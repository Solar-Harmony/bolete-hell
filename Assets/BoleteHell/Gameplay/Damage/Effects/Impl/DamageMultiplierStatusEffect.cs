using System;
using BoleteHell.Models;

namespace BoleteHell.Gameplay.Damage.Effects.Impl
{
    [Serializable]
    public sealed class DamageMultiplierStatusEffectConfig : StatusEffectConfig
    {
        public float damageMultiplier = 1.5f;
    }
    
    public class DamageMultiplierStatusEffect : IStatusEffect<DamageMultiplierStatusEffectConfig>
    {
        public bool CanApply(IStatusEffectTarget target, DamageMultiplierStatusEffectConfig config)
        {
            return target is IDamageDealer;
        }

        public void Apply(IStatusEffectTarget target, DamageMultiplierStatusEffectConfig config)
        {
            IDamageDealer damageDealer = (IDamageDealer)target;

            damageDealer.DamageMultiplier *= config.damageMultiplier;
        }

        public void Unapply(IStatusEffectTarget target, DamageMultiplierStatusEffectConfig config)
        {
            IDamageDealer damageDealer = (IDamageDealer)target;
           
            damageDealer.DamageMultiplier /= config.damageMultiplier;
        }
    }
}
