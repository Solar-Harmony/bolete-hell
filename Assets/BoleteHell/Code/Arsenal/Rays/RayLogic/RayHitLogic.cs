using System;
using BoleteHell.Code.Arsenal.RayData;
using BoleteHell.Code.Gameplay.Characters;
using BoleteHell.Code.Gameplay.Damage;
using BoleteHell.Code.Utils.LogFilter;
using UnityEngine;

namespace BoleteHell.Code.Arsenal.Rays.RayLogic
{
    [Serializable]
    public abstract class RayHitLogic
    {
        public static readonly LogCategory LogHits = new("Hit Logic", new Color(0.1f, 0.6f, 0.1f));
        
        public void OnHit(Vector2 hitPosition, IDamageable hitCharacterHealth, LaserInstance laserInstance, LaserData data)
        {
            FactionType hitCharacterFaction = ((IFaction)hitCharacterHealth).faction;
            float instigatorDamageMult = laserInstance.Instigator.GetDamageMultiplier(hitCharacterFaction);
            float laserDamageMult = ((IDamageDealer)laserInstance).GetDamageMultiplier(hitCharacterFaction);
            float damageMultiplier = instigatorDamageMult * laserDamageMult;
            int actualDamage = Mathf.RoundToInt(data.baseDamage * damageMultiplier);
            
            Scribe.Log(LogHits, $"{laserInstance.Instigator.GameObject.name} hit {((Character)hitCharacterHealth).name} for {actualDamage} damage (Base: {data.baseDamage}, Instigator Mult: {instigatorDamageMult}, Laser Mult: {laserDamageMult})");
            
            hitCharacterHealth.Health.TakeDamage(actualDamage);
            OnHitImpl(hitPosition, hitCharacterHealth, damageMultiplier);
        }

        public abstract void OnHitImpl(Vector2 hitPosition, IDamageable hitCharacterHealth, float damageMultiplier);
    }
}