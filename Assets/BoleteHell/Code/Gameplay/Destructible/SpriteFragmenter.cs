using UnityEngine;

namespace BoleteHell.Code.Gameplay.Destructible
{
    public class SpriteFragmenter : ISpriteFragmenter
    {
        public void Fragment(Vector2 position, SpriteFragmentConfig config)
        {
            Object.Instantiate(config.explosion, position, Quaternion.identity);

            var sprite = config.sr.sprite;
            var texture = sprite.texture;

            Vector2 spriteSize = sprite.bounds.size;
            var fragmentSize = new Vector2(spriteSize.x / config.fragmentsX, spriteSize.y / config.fragmentsY);
            var origin = position - spriteSize / 2;

            for (var y = 0; y < config.fragmentsY; y++)
            for (var x = 0; x < config.fragmentsX; x++)
            {
                var pos = origin + new Vector2((x + 0.5f) * fragmentSize.x, (y + 0.5f) * fragmentSize.y);

                var fragment = Object.Instantiate(config.fragmentPrefab);
                fragment.transform.position = pos;
                fragment.transform.rotation = config.parentRb.transform.rotation;
                fragment.transform.Translate(0, 0, -1.0f);

                var mesh = CreateQuadMesh(fragmentSize);
                var mf = fragment.GetComponent<MeshFilter>();
                mf.mesh = mesh;

                var uvWidth = sprite.rect.width / texture.width;
                var uvHeight = sprite.rect.height / texture.height;
                var uvX = (sprite.rect.x + x * sprite.rect.width / config.fragmentsX) / texture.width;
                var uvY = (sprite.rect.y + y * sprite.rect.height / config.fragmentsY) / texture.height;
                mf.mesh.uv = new Vector2[]
                {
                    new(uvX, uvY),
                    new(uvX + uvWidth / config.fragmentsX, uvY),
                    new(uvX, uvY + uvHeight / config.fragmentsY),
                    new(uvX + uvWidth / config.fragmentsX, uvY + uvHeight / config.fragmentsY)
                };
                
                var circleCollider = fragment.GetComponent<CircleCollider2D>();
                circleCollider.radius = fragmentSize.x / 4;

                var rb = fragment.GetComponent<Rigidbody2D>();
                rb.linearVelocity = config.parentRb.linearVelocity + Random.insideUnitCircle * config.explosionForce;
            }
        }

        private static Mesh CreateQuadMesh(Vector2 size)
        {
            var mesh = new Mesh();

            var vertices = new Vector3[]
            {
                new(-size.x / 2, -size.y / 2, 0),
                new(size.x / 2, -size.y / 2, 0),
                new(-size.x / 2, size.y / 2, 0),
                new(size.x / 2, size.y / 2, 0)
            };

            var triangles = new int[6] { 0, 2, 1, 2, 3, 1 };

            mesh.vertices = vertices;
            mesh.triangles = triangles;

            return mesh;
        }

        public void Fragment(System.Numerics.Vector2 position, SpriteFragmentConfig config)
        {
            throw new System.NotImplementedException();
        }
    }
}