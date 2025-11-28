using System;
using BoleteHell.Code.Arsenal.RayData;
using BoleteHell.Code.BoleteUtils.LogFilter;
using BoleteHell.Code.Gameplay.Characters;
using BoleteHell.Code.Gameplay.Damage;
using JetBrains.Annotations;
using UnityEngine;

namespace BoleteHell.Code.Arsenal.Rays.RayLogic
{
    [Serializable]
    public abstract class RayHitLogic
    {
        public static readonly LogCategory LogHits = new("Hit Logic", new Color(0.1f, 0.6f, 0.1f));
        
        /// <summary>
        /// Apply damage multipliers from instigator and victim to compute actual damage.
        /// </summary>
        /// <param name="baseDamage">How much dmg</param>
        /// <param name="victim">Who gets it</param>
        /// <param name="instigator">The laser weapon instance</param>
        protected int ComputeActualDamage(int baseDamage, GameObject victim, [CanBeNull] DamageDealerComponent instigator)
        {
            if (!victim.TryGetComponent(out FactionComponent faction))
                return baseDamage;
            
            float actualDamage = baseDamage;
            if (instigator)
            {
                actualDamage *= instigator.GetDamageMultiplier(faction.Type);
            }

            if (victim.TryGetComponent(out DamageDealerComponent damageDealer))
            {
                actualDamage *= damageDealer.GetDamageMultiplier(faction.Type);
            }
            
            return Mathf.RoundToInt(actualDamage);
        }
        
        public void OnHit(Vector2 hitPosition, HealthComponent victim, LaserInstance laser, LaserData laserData)
        {
            var damageDealer = laser.Instigator.GetComponent<DamageDealerComponent>();
            int damage = ComputeActualDamage(laserData.baseDamage, victim.gameObject, damageDealer);
 
            Scribe.Log(LogHits, "{0} was hit by {1} for {2}hp and lost {3}hp.",
                victim.gameObject.name,
                laser.Instigator.name,
                laserData.baseDamage,
                damage);
            
            victim.TakeDamage(damage);
            OnHitImpl(hitPosition, victim, laser);
        }

        public abstract void OnHitImpl(Vector2 hitPosition, HealthComponent victim, LaserInstance laser);
    }
}