using System;
using BoleteHell.Code.Gameplay.Damage.Effects;
using BoleteHell.Gameplay.Characters;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BoleteHell.Gameplay.StatusEffectImpl
{
    [Serializable]
    public sealed class DamageMultiplierStatusEffectConfig : StatusEffectConfig
    {
        public float DamageMultiplier = 1.5f;

        /// <summary>
        /// The faction that will take the bonus damage, set to null if general
        /// So a bullet can deal more damage if it hits specifically a shroom, or specifically the player
        /// </summary>
        public bool IsAppliedToFaction;
        [ShowIf("IsAppliedToFaction")][Tooltip("The faction that will take the bonus damage (if Enemy, damage against characters of enemy faction will take more damage)")]
        public FactionType AffectedFaction;
    }
    
    public class DamageMultiplierStatusEffect : IStatusEffect<DamageMultiplierStatusEffectConfig>
    {
        public bool CanApply(GameObject target, DamageMultiplierStatusEffectConfig config)
        {
            return target.TryGetComponent<DamageDealerComponent>(out _);
        }

        public void Apply(GameObject target, DamageMultiplierStatusEffectConfig config)
        {
            DamageDealerComponent damageDealer = target.GetComponent<DamageDealerComponent>();
            FactionType? faction = config.IsAppliedToFaction ? config.AffectedFaction : null;
            damageDealer.AddDamageMultiplier(faction, config.DamageMultiplier);
        }

        public void Unapply(GameObject target, DamageMultiplierStatusEffectConfig config)
        {
            DamageDealerComponent damageDealer = target.GetComponent<DamageDealerComponent>();
            FactionType? faction = config.IsAppliedToFaction ? config.AffectedFaction : null;
            damageDealer.RemoveDamageMultiplier(faction, config.DamageMultiplier);
        }
    }
}
