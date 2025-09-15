using System;
using BoleteHell.Arsenals.LaserData;
using BoleteHell.Arsenals.Rays;
using BoleteHell.Code.Audio.BoleteHell.Models;
using BoleteHell.Utils;
using UnityEngine;

namespace BoleteHell.Gameplay.Lasers
{
    [Serializable]
    public abstract class RayHitLogic : IOnHitLogic
    {
        //Au lieu de health on pourrais avoir un component de stats en général comme ça les tir pourrait affecter le stat qu'il veut directement
        //(réduire le mouvement, reduire l'attaque wtv)
        public void OnHit(Vector2 hitPosition, IDamageable hitCharacterHealth, LaserInstance laserInstance, LaserData data)
        {
            ServiceLocator.Inject(this);
            Debug.Log($"dealt {(int)(data.baseDamage * laserInstance.DamageMultiplier)} damage");
            hitCharacterHealth.Health.TakeDamage((int)(data.baseDamage * laserInstance.DamageMultiplier));
            
            OnHitImpl(hitPosition, hitCharacterHealth);
        }

        public abstract void OnHitImpl(Vector2 hitPosition, IDamageable hitCharacterHealth);
    }
}