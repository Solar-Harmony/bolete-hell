using System;
using UnityEngine;

namespace BoleteHell.Code.Utils
{
    public interface IInstantiator
    {
        public T CloneScriptableObject<T>(T original) where T : ScriptableObject;
        public void InstantiateThenDestroyLater(GameObject prefab, Vector2 position, Quaternion rotation, float timeToDestroy, Action<GameObject> initCallback = null);
    }
}