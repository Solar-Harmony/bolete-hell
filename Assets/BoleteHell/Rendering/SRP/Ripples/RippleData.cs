using UnityEngine;

namespace BoleteHell.Rendering.Ripples
{
    public struct RippleData
    {
        public Vector2 Position;
        public float SpawnTime;
        public float Intensity;

        public RippleData(Vector2 position, float spawnTime, float intensity)
        {
            Position = position;
            SpawnTime = spawnTime;
            Intensity = intensity;
        }

        public Vector4 ToVector4() => new(Position.x, Position.y, SpawnTime, Intensity);

        public float Age => Time.time - SpawnTime;

        public bool IsExpired(float lifetime) => Age > lifetime;
    }
}


