using System;
using BoleteHell.Code.Arsenal.RayData;
using BoleteHell.Code.Gameplay.Characters;
using BoleteHell.Code.Gameplay.Damage;
using UnityEngine;

namespace BoleteHell.Code.Arsenal.Rays.RayLogic
{
    [Serializable]
    public abstract class RayHitLogic
    {
        public void OnHit(Vector2 hitPosition, IDamageable hitCharacterHealth, LaserInstance laserInstance, LaserData data)
        {
            FactionType hitCharacterFaction = ((IFaction)hitCharacterHealth).faction;
            
            float characterDamageMultiplierAgainstTarget = laserInstance.Instigator.GetDamageMultiplier(hitCharacterFaction);
            
            float laserDamageMultiplierAgainstTarget =
                ((IDamageDealer)laserInstance).GetDamageMultiplier(hitCharacterFaction);
            
            hitCharacterHealth.Health.TakeDamage((int)(data.baseDamage * characterDamageMultiplierAgainstTarget * laserDamageMultiplierAgainstTarget));
            
            OnHitImpl(hitPosition, hitCharacterHealth);
        }

        public abstract void OnHitImpl(Vector2 hitPosition, IDamageable hitCharacterHealth);
    }
}