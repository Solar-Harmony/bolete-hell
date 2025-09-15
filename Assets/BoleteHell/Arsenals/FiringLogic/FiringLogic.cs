using System;
using BoleteHell.Arsenals.Cannons;
using BoleteHell.Arsenals.HitHandler;
using BoleteHell.Arsenals.LaserData;
using BoleteHell.Arsenals.Rays;
using UnityEngine;

namespace BoleteHell.Arsenals.FiringLogic
{
    public abstract class FiringLogic : ITargetable
    {
        protected Vector2 CurrentDirection;
        protected Vector3 CurrentPos;
    
        public abstract void Shoot(Vector3 bulletSpawnPoint, Vector2 direction, CannonData data, LaserCombo laserCombo, GameObject instigator = null);
        public virtual void OnHit(ITargetable.Context ctx, Action<ITargetable.Response> callback = null)
        {
            // always ignore hits with the instigator (for now)
            if (ctx.HitObject == ctx.Instigator)
                return;

            ITargetable handler = ctx.HitObject.GetComponent<ITargetable>()
                                  ?? ctx.HitObject.GetComponentInParent<ITargetable>(); // TODO : needed because of shield, child colliders are not registered to composite collider correctly but i couldn't get it working

            handler?.OnHit(ctx, response =>
            {
                if (response.RequestDestroyProjectile )
                {
                    LaserInstance laserInstance = ctx.Projectile;
                   
                    if(laserInstance.isProjectile)
                        laserInstance.ResetLaser();
                }
            
                callback?.Invoke(response);
            });
        }
    
        public abstract void FinishFiring();
    }
}
