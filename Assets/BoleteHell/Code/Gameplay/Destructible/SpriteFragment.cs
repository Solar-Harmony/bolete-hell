using System;
using BoleteHell.Utils;
using JetBrains.Annotations;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace BoleteHell.Code.Gameplay.Destructible
{
    public record SpriteFragmentParams(
        int x,
        int y,
        Vector2 fragmentSize,
        Vector3 originalLocalScale,
        float timeToDestroy
    );
    
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(CircleCollider2D))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class SpriteFragment : MonoBehaviour, IPoolable<Vector2, SpriteFragmentConfig, SpriteFragmentParams>
    {
        private float _timeToDestroy;
        
        private event Action OnInitialized;
        
        public void OnSpawned(Vector2 position, SpriteFragmentConfig config, SpriteFragmentParams parameters)
        {
            transform.position = position;
            transform.rotation = config.parentRb.transform.rotation;
            transform.Translate(0, 0, -1.0f);
            
            var sprite = config.sr.sprite;
            var texture = sprite.texture;
            var spriteRenderer = GetComponent<Renderer>();
            spriteRenderer.material.mainTexture = texture;
            transform.localScale = parameters.originalLocalScale;

            var mesh = CreateQuadMesh(parameters.fragmentSize);
            var mf = GetComponent<MeshFilter>();
            mf.mesh = mesh;
            
            var uvWidth = sprite.rect.width / texture.width;
            var uvHeight = sprite.rect.height / texture.height;
            var uvX = (sprite.rect.x + parameters.x * sprite.rect.width / config.fragmentsX) / texture.width;
            var uvY = (sprite.rect.y + parameters.y * sprite.rect.height / config.fragmentsY) / texture.height;
            mf.mesh.uv = new Vector2[]
            {
                new(uvX, uvY),
                new(uvX + uvWidth / config.fragmentsX, uvY),
                new(uvX, uvY + uvHeight / config.fragmentsY),
                new(uvX + uvWidth / config.fragmentsX, uvY + uvHeight / config.fragmentsY)
            };
            
            var circleCollider = GetComponent<CircleCollider2D>();
            circleCollider.radius = parameters.fragmentSize.x / 4;

            var rb = GetComponent<Rigidbody2D>();
            rb.linearVelocity = config.parentRb.linearVelocity + Random.insideUnitCircle * config.explosionForce;
            
            _timeToDestroy = parameters.timeToDestroy;
            
            OnInitialized?.Invoke();
        }

        public void OnDespawned()
        {

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

            var triangles = new[] { 0, 2, 1, 2, 3, 1 };

            mesh.vertices = vertices;
            mesh.triangles = triangles;

            return mesh;
        }
        
        [UsedImplicitly]
        public class Pool : MonoPoolableMemoryPool<Vector2, SpriteFragmentConfig, SpriteFragmentParams, SpriteFragment>
        {
            [Inject]
            private IObjectInstantiator _instantiator;
            
            protected override void OnSpawned(SpriteFragment item)
            {
                Action onInitializedHandler = null;
                onInitializedHandler = () =>
                {
                    item.OnInitialized -= onInitializedHandler;
                    _instantiator.DespawnLater(this, item, item._timeToDestroy);
                };
                item.OnInitialized += onInitializedHandler;
            }
        }
    }
}
