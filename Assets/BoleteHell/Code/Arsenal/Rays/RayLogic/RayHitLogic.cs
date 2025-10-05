using System;
using BoleteHell.Code.Arsenal.RayData;
using BoleteHell.Code.Gameplay.Characters;
using BoleteHell.Code.Gameplay.Damage;
using UnityEngine;

namespace BoleteHell.Code.Arsenal.Rays.RayLogic
{
    // TODO: Use a similar pattern to the status effects to avoid injection in hit logics
    [Serializable]
    public abstract class RayHitLogic : IRequestManualInject
    {
        bool IRequestManualInject.IsInjected { get; set; } = false;
        
        // TODO: Have a stats component that handles damage multipliers, movement speed, and other stats?
        public void OnHit(Vector2 hitPosition, IDamageable hitCharacterHealth, LaserInstance laserInstance, LaserData data)
        {
            ((IRequestManualInject)this).InjectDependencies();
            
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