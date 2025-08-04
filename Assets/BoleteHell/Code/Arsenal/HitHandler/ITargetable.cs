using System;
using UnityEngine;

namespace BoleteHell.Code.Arsenal.HitHandler
{
    public interface ITargetable
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
            bool RequestDestroyProjectile
        )
        {
            public Response(Context ctx) : this(ctx.Position, ctx.Direction, false) {}
        }

        public void OnHit(Context ctx, Action<Response> callback = null);
    }
}