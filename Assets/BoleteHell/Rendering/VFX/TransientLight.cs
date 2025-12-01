using BoleteHell.Utils;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Zenject;

namespace BoleteHell.Code.Graphics
{
    /// <summary>
    /// A light that spawns and despawns within a short time.
    /// </summary>
    [RequireComponent(typeof(Light2D))]
    public class TransientLight : MonoBehaviour, IPoolable<Vector2, float, float>
    {
        private float _timeToDestroy;
        
        public void OnSpawned(Vector2 position, float radius, float timeToDestroy)
        {
            transform.position = position;
            
            var pointLight = GetComponent<Light2D>();
            pointLight.pointLightOuterRadius = radius;
            _timeToDestroy = timeToDestroy;
            
            gameObject.SetActive(true);
        }

        public void OnDespawned()
        {
        }
        
        // Vector2 position, float radius, float timeToDestroy
        public class Pool : MonoPoolableMemoryPool<Vector2, float, float, TransientLight>
        {
            [Inject]
            private IObjectInstantiator _instantiator;

            protected override void OnSpawned(TransientLight item)
            {
                base.OnSpawned(item);
                _instantiator.DespawnLater(this, item, item._timeToDestroy);
            }
        }
    }
}
