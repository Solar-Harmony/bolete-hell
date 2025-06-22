using UnityEngine;

namespace _BoleteHell.Code.ProjectileSystem.HitHandler
{
    public class Wall : MonoBehaviour, IHitHandler
    {
        public IHitHandler.Output OnHit(IHitHandler.Context ctx)
        {
            // TODO improve
            if (ctx.Projectile && ctx.Projectile.TryGetComponent(out LaserProjectileMovement projectile))
            {
                projectile.DestroyProjectile();
            }
            
            return new IHitHandler.Output(
                Position: ctx.Position,
                Direction: ctx.Direction 
            );
        }
    }
}