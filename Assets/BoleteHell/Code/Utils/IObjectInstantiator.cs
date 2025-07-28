using System;
using UnityEngine;
using Zenject;

namespace BoleteHell.Code.Utils
{
    public interface IObjectInstantiator
    {
        /// <summary>
        /// Allows cloning of ScriptableObjects in classes that are not MonoBehaviours.
        /// </summary>
        /// <param name="original"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T CloneScriptableObject<T>(T original) where T : ScriptableObject;
        
        /// <summary>
        /// Non-pooled version. Consider subclassing IMemoryPool<T> for pooled instantiation.
        /// </summary>
        /// <param name="prefab"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <param name="timeToDestroy"></param>
        /// <param name="initCallback"></param>
        public void InstantiateThenDestroyLater(GameObject prefab, Vector2 position, Quaternion rotation, float timeToDestroy, Action<GameObject> initCallback = null);

        /// <summary>
        /// Only used in pools, TODO: move to separate interface.
        /// </summary>
        /// <param name="pool"></param>
        /// <param name="item"></param>
        /// <param name="delay"></param>
        public void DespawnLater(IMemoryPool pool, object item, float delay);
    }
}