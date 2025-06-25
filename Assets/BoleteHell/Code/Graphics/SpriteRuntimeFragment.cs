using UnityEngine;

namespace Graphics
{
    public class SpriteRuntimeFragmenter : MonoBehaviour
    {
        public int fragmentsX = 4;
        public int fragmentsY = 4;
        public float explosionForce = 5f;

        [SerializeField] private SpriteRenderer sr;
        [SerializeField] private Rigidbody2D parentRb;
        [SerializeField] private GameObject explosion;
        [SerializeField] private GameObject fragmentPrefab;

        public void Fragment()
        {
            Instantiate(explosion, transform.position, Quaternion.identity);

            var sprite = sr.sprite;
            var texture = sprite.texture;

            Vector2 spriteSize = sprite.bounds.size;
            var fragmentSize = new Vector2(spriteSize.x / fragmentsX, spriteSize.y / fragmentsY);
            var origin = (Vector2)transform.position - spriteSize / 2;

            for (var y = 0; y < fragmentsY; y++)
            for (var x = 0; x < fragmentsX; x++)
            {
                var pos = origin + new Vector2((x + 0.5f) * fragmentSize.x, (y + 0.5f) * fragmentSize.y);

                var fragment = Instantiate(fragmentPrefab);
                fragment.transform.position = pos;
                fragment.transform.rotation = parentRb.transform.rotation;
                fragment.transform.Translate(0, 0, -1.0f);

                var mesh = CreateQuadMesh(fragmentSize);
                var mf = fragment.GetComponent<MeshFilter>();
                mf.mesh = mesh;

                var uvWidth = sprite.rect.width / texture.width;
                var uvHeight = sprite.rect.height / texture.height;
                var uvX = (sprite.rect.x + x * sprite.rect.width / fragmentsX) / texture.width;
                var uvY = (sprite.rect.y + y * sprite.rect.height / fragmentsY) / texture.height;
                mf.mesh.uv = new Vector2[]
                {
                    new(uvX, uvY),
                    new(uvX + uvWidth / fragmentsX, uvY),
                    new(uvX, uvY + uvHeight / fragmentsY),
                    new(uvX + uvWidth / fragmentsX, uvY + uvHeight / fragmentsY)
                };
                
                var circleCollider = fragment.GetComponent<CircleCollider2D>();
                circleCollider.radius = fragmentSize.x / 4;

                var rb = fragment.GetComponent<Rigidbody2D>();
                rb.linearVelocity = parentRb.linearVelocity + Random.insideUnitCircle * explosionForce;
            }

            gameObject.SetActive(false);
            Destroy(gameObject);
        }

        private Mesh CreateQuadMesh(Vector2 size)
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
    }
}