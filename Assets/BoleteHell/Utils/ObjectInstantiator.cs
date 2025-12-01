using System;
using System.Collections;
using JetBrains.Annotations;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

namespace BoleteHell.Utils
{
    [UsedImplicitly]
    public class ObjectInstantiator : IObjectInstantiator
    {
        [Inject]
        private ICoroutineProvider _coroutine;

        [Inject]
        private DiContainer _container;
        
        public void InstantiateThenDestroyLater(GameObject prefab, Vector2 position, Quaternion rotation, float timeToDestroy, Action<GameObject> initCallback = null)
        {
            Debug.Assert(timeToDestroy >= 0.0f);
            
            GameObject obj = Object.Instantiate(prefab, position, rotation);
            initCallback?.Invoke(obj);
            Object.Destroy(obj, timeToDestroy); 
        }
        
        public void DespawnLater(IMemoryPool pool, object item, float delay)
        {
            Debug.Assert(delay >= 0.0f);
            _coroutine.StartCoroutine(WaitThenReturnToPool(pool, item, delay));
        }

        public GameObject InstantiateWithInjection(GameObject prefab)
        {
           return _container.InstantiatePrefab(prefab);
        }

        public GameObject InstantiateWithInjection(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent)
        {
            return _container.InstantiatePrefab(prefab, position, rotation, parent);
        }

        private static IEnumerator WaitThenReturnToPool(IMemoryPool pool, object item, float delay)
        {
            yield return new WaitForSeconds(delay);
            pool.Despawn(item);
        }
    }
}
