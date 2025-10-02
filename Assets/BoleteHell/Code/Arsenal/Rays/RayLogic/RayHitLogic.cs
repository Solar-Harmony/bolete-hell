using System;
using BoleteHell.Code.Arsenal.RayData;
using BoleteHell.Code.Gameplay.Damage;
using UnityEngine;

namespace BoleteHell.Code.Arsenal.Rays.RayLogic
{
    [Serializable]
    public abstract class RayHitLogic
    {
        public void OnHit(Vector2 hitPosition, IDamageable hitCharacterHealth, LaserInstance laserInstance, LaserData data)
        {
            hitCharacterHealth.Health.TakeDamage((int)(data.baseDamage * laserInstance.DamageMultiplier));
            OnHitImpl(hitPosition, hitCharacterHealth);
        }

        public abstract void OnHitImpl(Vector2 hitPosition, IDamageable hitCharacterHealth);
    }
}