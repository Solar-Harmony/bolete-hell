using System;
using _BoleteHell.Code.ProjectileSystem.HitHandler;
using BoleteHell.Rays;
using Data.Cannons;
using Data.Rays;
using Lasers;
using UnityEngine;
using Object = UnityEngine.Object;

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
                    LaserRendererPool.Instance.Release(renderer);
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
