using System;
using BoleteHell.Code.Gameplay.Characters;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BoleteHell.Code.Gameplay.Damage.Effects.Impl
{
    [Serializable]
    public sealed class DamageMultiplierStatusEffectConfig : StatusEffectConfig
    {
        public float damageMultiplier = 1.5f;

        /// <summary>
        /// The faction that will take the bonus damage, set to null if general
        /// So a bullet can deal more damage if it hits specifically a shroom, or specifically the player
        /// </summary>
        public bool isAppliedToFaction;
        [ShowIf("isAppliedToFaction")][Tooltip("The faction that will take the bonus damage (if Enemy, damage against characters of enemy faction will take more damage)")]
        public FactionType affectedFaction;
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
            FactionType? faction = config.isAppliedToFaction ? config.affectedFaction : null;
            damageDealer.AddDamageMultiplier(faction, config.damageMultiplier);
        }

        public void Unapply(IStatusEffectTarget target, DamageMultiplierStatusEffectConfig config)
        {
            IDamageDealer damageDealer = (IDamageDealer)target;
            FactionType? faction = config.isAppliedToFaction ? config.affectedFaction : null;
            damageDealer.RemoveDamageMultiplier(faction, config.damageMultiplier);
        }
    }
}
