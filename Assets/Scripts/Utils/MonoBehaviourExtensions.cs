using System.Runtime.CompilerServices;
using UnityEngine;

namespace Utils
{
    public static class MonoBehaviourExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AssertNotNull<T>(this MonoBehaviour script, T member) where T : Object
        {
            if (!member)
            {
                string thisType = script.GetType().Name;
                string memberType = typeof(T).Name;
                Debug.LogError($"Script {thisType} disabled because {memberType} is not assigned in the inspector.", script);
                script.enabled = false;
            }
        }
    }
}