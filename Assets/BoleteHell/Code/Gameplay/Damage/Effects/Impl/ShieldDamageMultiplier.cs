using System;
using BoleteHell.Code.Arsenal.Rays.RayLogic;
using UnityEngine;

namespace BoleteHell.Code.Gameplay.Damage.Effects.Impl
{
    [Serializable]
    public sealed class DamageStatusEffectConfig : ShieldEffect
    {
        public float damageMultiplier = 1.5f;
    }
    
    public class ShieldDamageMultiplier : IStatusEffect<DamageStatusEffectConfig>
    {
        public bool CanApply(IStatusEffectTarget target, DamageStatusEffectConfig config)
        {
            return target is RayHitLogic;
        }

        public void Apply(IStatusEffectTarget target, DamageStatusEffectConfig config)
        {
            RayHitLogic laser = (RayHitLogic)target;
           
            laser.baseHitDamage = (int)(laser.baseHitDamage * config.damageMultiplier);
        }

        public void Unapply(IStatusEffectTarget target, DamageStatusEffectConfig config)
        {
            RayHitLogic laser = (RayHitLogic)target;
           
            laser.baseHitDamage = (int)(laser.baseHitDamage / config.damageMultiplier);
        }
    }
}
