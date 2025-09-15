using System;
using BoleteHell.Arsenals.Rays;
using UnityEngine;

namespace BoleteHell.Arsenals.HitHandler
{
    public interface ITargetable
    {
        public record Context
        (
            GameObject HitObject,
            GameObject Instigator,
            LaserInstance Projectile,
            Vector2 Position,
            Vector2 Direction,
            IProjectileData Data
        );

        public record Response(
            Vector2 Position, 
            Vector2 Direction,
            bool RequestDestroyProjectile
        )
        {
            public Response(Context ctx) : this(ctx.Position, ctx.Direction, false) {}
        }

        public void OnHit(Context ctx, Action<Response> callback = null);
    }
}