namespace BoleteHell.Graphics
{
    using UnityEngine;

    public class SpriteRuntimeFragmenter : MonoBehaviour
    {
        public int fragmentsX = 4;
        public int fragmentsY = 4;
        public float explosionForce = 5f;
        public Material material;

        [SerializeField] 
        private SpriteRenderer sr;

        [SerializeField] 
        private Rigidbody2D parentRb;

        [SerializeField] 
        private GameObject explosion;

        [SerializeField] 
        private GameObject fragmentPrefab;

        public void Fragment(Vector2 impactNormal)
        {
            if (sr == null || parentRb == null)
            {
                Debug.LogError("SpriteRenderer or Rigidbody2D is not assigned.");
                return;
            }

            Instantiate(explosion, transform.position, Quaternion.identity);

            Sprite sprite = sr.sprite;
            Texture2D texture = sprite.texture;

            Vector2 spriteSize = sprite.bounds.size;
            Vector2 fragmentSize = new Vector2(spriteSize.x / fragmentsX, spriteSize.y / fragmentsY);
            Vector2 origin = (Vector2)transform.position - spriteSize / 2;

            for (int y = 0; y < fragmentsY; y++)
            {
                for (int x = 0; x < fragmentsX; x++)
                {
                    Vector2 pos = origin + new Vector2((x + 0.5f) * fragmentSize.x, (y + 0.5f) * fragmentSize.y);

                    GameObject fragment = Instantiate(fragmentPrefab);
                    fragment.transform.position = pos;
                    fragment.transform.rotation = parentRb.transform.rotation;
                    fragment.transform.Translate(0, 0, -1.0f);

                    Mesh mesh = CreateQuadMesh(fragmentSize);
                    MeshFilter mf = fragment.GetComponent<MeshFilter>();
                    mf.mesh = mesh;

                    // UV Mapping
                    float uvWidth = sprite.rect.width / texture.width;
                    float uvHeight = sprite.rect.height / texture.height;
                    float uvX = (sprite.rect.x + x * sprite.rect.width / fragmentsX) / texture.width;
                    float uvY = (sprite.rect.y + y * sprite.rect.height / fragmentsY) / texture.height;
                    mf.mesh.uv = new Vector2[]
                    {
                        new(uvX, uvY),
                        new(uvX + uvWidth / fragmentsX, uvY),
                        new(uvX, uvY + uvHeight / fragmentsY),
                        new(uvX + uvWidth / fragmentsX, uvY + uvHeight / fragmentsY)
                    };

                    Rigidbody2D rb = fragment.GetComponent<Rigidbody2D>();

                    CircleCollider2D skibiid = fragment.GetComponent<CircleCollider2D>();
                    skibiid.radius = fragmentSize.x / 4;

                    rb.linearVelocity = parentRb.linearVelocity + Random.insideUnitCircle * explosionForce;
                }
            }

            Destroy(gameObject);
        }

        private Mesh CreateQuadMesh(Vector2 size)
        {
            Mesh mesh = new Mesh();

            Vector3[] vertices = new Vector3[4]
            {
                new Vector3(-size.x / 2, -size.y / 2, 0),
                new Vector3(size.x / 2, -size.y / 2, 0),
                new Vector3(-size.x / 2, size.y / 2, 0),
                new Vector3(size.x / 2, size.y / 2, 0)
            };

            int[] triangles = new int[6] { 0, 2, 1, 2, 3, 1 };

            mesh.vertices = vertices;
            mesh.triangles = triangles;
            // mesh.RecalculateNormals();

            return mesh;
        }
    }
}