using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace BoleteHell.Code.Utils
{
    public class ObjectInstantiator : IObjectInstantiator
    {
        /// <summary>
        /// Allows cloning of ScriptableObjects in classes that are not MonoBehaviours.
        /// </summary>
        /// <param name="original"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
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
    }
}
