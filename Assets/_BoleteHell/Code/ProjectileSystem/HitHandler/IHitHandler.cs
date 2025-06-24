using System;
using BoleteHell.Rays;
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
    }
}