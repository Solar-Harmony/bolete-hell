using System;
using System.Collections;
using JetBrains.Annotations;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

namespace BoleteHell.Code.Utils
{
    [UsedImplicitly]
    public class ObjectInstantiator : IObjectInstantiator
    {
        public T CloneScriptableObject<T>(T original) where T : ScriptableObject
        {
            if (!original)
            {
                Debug.LogError("Tried to clone a null object");
            }

            T clone = Object.Instantiate(original);
            return clone;
        }
        
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
            GlobalCoroutine.Launch(WaitThenReturnToPool(pool, item, delay));
        }
        
        private static IEnumerator WaitThenReturnToPool(IMemoryPool pool, object item, float delay)
        {
            yield return new WaitForSeconds(delay);
            pool.Despawn(item);
        }
    }
}
