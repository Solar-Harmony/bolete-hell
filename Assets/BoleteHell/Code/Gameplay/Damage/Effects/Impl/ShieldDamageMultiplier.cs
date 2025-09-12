using System;
using BoleteHell.Code.Arsenal.Rays.RayLogic;
using UnityEngine;

namespace BoleteHell.Code.Gameplay.Damage.Effects.Impl
{
    [Serializable]
    public sealed class DamageStatusEffectConfig : StatusEffectConfig
    {
        public float damageMultiplier = 1.5f;
    }
    
    public class ShieldDamageMultiplier : IStatusEffect<DamageStatusEffectConfig>
    {
        public bool CanApply(IStatusEffectTarget target, DamageStatusEffectConfig config)
        {
            return target is IDamageDealer;
        }

        public void Apply(IStatusEffectTarget target, DamageStatusEffectConfig config)
        {
            IDamageDealer damageDealer = (IDamageDealer)target;

            damageDealer.DamageMultiplier *= config.damageMultiplier;
        }

        public void Unapply(IStatusEffectTarget target, DamageStatusEffectConfig config)
        {
            IDamageDealer damageDealer = (IDamageDealer)target;
           
            damageDealer.DamageMultiplier /= config.damageMultiplier;
        }
    }
}
