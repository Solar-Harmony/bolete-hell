using UnityEngine;

namespace _BoleteHell.Code.ProjectileSystem.HitHandler
{
    public interface IHitHandler
    {
        public class Context
        {
            public GameObject Instigator { get; }
            public GameObject Projectile { get; }
            public Vector2 Position { get; }
            public Vector2 Direction { get; }
            public IProjectileData Data { get; }
            
            public Context(GameObject instigator, GameObject projectile, Vector2 position, Vector2 direction, IProjectileData data)
            {
                Instigator = instigator;
                Projectile = projectile;
                Position = position;
                Direction = direction;
                Data = data;
            }
        }

        void OnHit(Context ctx);
    }
}