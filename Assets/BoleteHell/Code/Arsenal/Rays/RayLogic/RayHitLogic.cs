using System;
using BoleteHell.Code.Arsenal.RayData;
using BoleteHell.Code.Gameplay.Characters;
using BoleteHell.Code.Gameplay.Damage;
using BoleteHell.Code.Utils.LogFilter;
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
        protected int ComputeActualDamage(int baseDamage, IDamageable victim, [CanBeNull] IInstigator instigator)
        {
            if (victim is not IFaction faction)
                return baseDamage;
            
            float actualDamage = baseDamage;
            if (instigator != null)
            {
                actualDamage *= instigator.GetDamageMultiplier(faction.faction);
            }

            if (victim is IDamageDealer damageDealer)
            {
                actualDamage *= damageDealer.GetDamageMultiplier(faction.faction);
            }
            
            return Mathf.RoundToInt(actualDamage);
        }
        
        public void OnHit(Vector2 hitPosition, IDamageable victim, LaserInstance laser, LaserData laserData)
        {
            int damage = ComputeActualDamage(laserData.baseDamage, victim, laser.Instigator);
 
            Scribe.Log(LogHits, "{0} was hit by {1} for {2}hp and lost {3}hp.",
                victim.Health.gameObject.name,
                laser.Instigator.GameObject.name,
                laserData.baseDamage,
                damage);
            
            victim.Health.TakeDamage(damage);
            OnHitImpl(hitPosition, victim, laser);
        }

        public abstract void OnHitImpl(Vector2 hitPosition, IDamageable victim, LaserInstance laser);
    }
}