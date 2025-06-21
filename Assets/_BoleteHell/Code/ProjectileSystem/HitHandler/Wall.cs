using UnityEngine;

namespace _BoleteHell.Code.ProjectileSystem.HitHandler
{
    public class Wall : MonoBehaviour, IHitHandler
    {
        public void OnHit(IHitHandler.Context ctx)
        {
            // TODO improve
            if (ctx.Source.TryGetComponent(out LaserProjectileMovement projectile))
            {
                projectile.DestroyProjectile();
            }
        }
    }
}