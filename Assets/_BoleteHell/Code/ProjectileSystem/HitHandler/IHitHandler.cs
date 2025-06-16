using UnityEngine;

namespace _BoleteHell.Code.ProjectileSystem.HitHandler
{
    public interface IHitHandler
    {
        public class Context
        {
            public GameObject Source { get; }
            public Vector2 Position { get; }
            public Vector2 Direction { get; }
            public IProjectileData Data { get; }
            
            public Context(GameObject source, Vector2 position, Vector2 direction, IProjectileData data)
            {
                Source = source;
                Position = position;
                Direction = direction;
                Data = data;
            }
        }

        void OnHit(Context ctx);
    }
}