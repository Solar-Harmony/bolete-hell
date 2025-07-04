using System;
using BoleteHell.Code.Arsenal.Cannons;
using BoleteHell.Code.Arsenal.HitHandler;
using BoleteHell.Code.Arsenal.RayData;
using BoleteHell.Code.Arsenal.Rays;
using UnityEngine;
using Object = UnityEngine.Object;

namespace BoleteHell.Code.Arsenal.FiringLogic
{
    public abstract class FiringLogic : IHitHandler
    {
        protected Vector2 CurrentDirection;
        protected Vector3 CurrentPos;
    
        public abstract void Shoot(Vector3 bulletSpawnPoint, Vector2 direction, CannonData data, LaserCombo laserCombo, GameObject instigator = null);
        public virtual void OnHit(IHitHandler.Context ctx, Action<IHitHandler.Response> callback = null)
        {
            // always ignore hits with the instigator (for now)
            if (ctx.HitObject == ctx.Instigator)
                return;

            IHitHandler handler = ctx.HitObject.GetComponent<IHitHandler>()
                                  ?? ctx.HitObject.GetComponentInParent<IHitHandler>(); // TODO : needed because of shield, child colliders are not registered to composite collider correctly but i couldn't get it working

            handler?.OnHit(ctx, response =>
            {
                if (response.RequestDestroy)
                {
                    LaserRenderer renderer = ctx.Projectile?.GetComponent<LaserRenderer>();
                    if (renderer)
                    {
                        ctx.Projectile.gameObject.GetComponent<LaserRenderer>().ResetLaser();
                    }
                    else
                    {
                        Object.Destroy(ctx.Projectile); 
                    }
                }
            
                callback?.Invoke(response);
            });
        }
    
        public abstract void FinishFiring();
    }
}
