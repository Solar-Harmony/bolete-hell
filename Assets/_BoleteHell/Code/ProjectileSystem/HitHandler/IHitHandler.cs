using System;
using UnityEngine;

namespace _BoleteHell.Code.ProjectileSystem.HitHandler
{
    public interface IHitHandler
    {
        public record Context
        (
            GameObject HitObject,
            GameObject Instigator,
            GameObject Projectile,
            Vector2 Position,
            Vector2 Direction,
            IProjectileData Data
        );

        public record Response(
            Vector2 Position, 
            Vector2 Direction,
            bool RequestDestroy
        )
        {
            public Response(Context ctx) : this(ctx.Position, ctx.Direction, false) {}
        }

        public void OnHit(Context ctx, Action<Response> callback = null);

        public static void TryHandleHit(Context ctx, Action<Response> callback = null)
        {
            // always ignore hits with the instigator (for now)
            if (ctx.HitObject == ctx.Instigator)
                return;

            IHitHandler handler = ctx.HitObject.GetComponent<IHitHandler>()
                                  ?? ctx.HitObject.GetComponentInParent<IHitHandler>(); // TODO : needed because of shield, child colliders are not registered to composite collider correctly but i couldn't get it working

            handler?.OnHit(ctx, callback);
        }
    }
}