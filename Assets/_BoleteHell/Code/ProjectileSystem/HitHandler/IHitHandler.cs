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

        public record Output(Vector2 Position, Vector2 Direction)
        {
            public Output(Context ctx) : this(ctx.Position, ctx.Direction) {}
        }

        Output OnHit(Context ctx);

        public static Output TryHandleHit(Context ctx)
        {
            // always ignore hits with the instigator (for now)
            if (ctx.HitObject == ctx.Instigator)
                return new Output(ctx);

            IHitHandler handler = ctx.HitObject.GetComponent<IHitHandler>()
                                  ?? ctx.HitObject.GetComponentInParent<IHitHandler>(); // TODO : needed because of shield, child colliders are not registered to composite collider correctly but i couldn't get it working

            return handler?.OnHit(ctx);
        }
    }
}