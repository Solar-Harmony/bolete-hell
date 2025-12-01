using System;
using UnityEngine;
using Zenject;

namespace BoleteHell.Utils
{
    public interface IObjectInstantiator
    {
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


        public GameObject InstantiateWithInjection(GameObject prefab);
        public GameObject InstantiateWithInjection(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent);
    }
}