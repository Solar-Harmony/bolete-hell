using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;
using Zenject;
using Debug = UnityEngine.Debug;

namespace BoleteHell.Code.Core
{
    /// <summary>
    /// Service locator for dependency resolution in contexts where proper DI isn't possible.
    /// Only use this when strictly needed. Will fail on purpose if used from a MonoBehaviour or SO.
    /// </summary>
    public class ServiceLocator
    {
        private static DiContainer _container;
        private static readonly Dictionary<Type, object> Cache = new();

        internal static void Initialize(DiContainer container)
        {
            _container = container;
        }
        
        public static void ClearCache()
        {
            Cache.Clear();
        }

        public static void Get<T>(ref T obj)
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            ValidateCallingSite<T>();
#endif

            if (Cache.TryGetValue(typeof(T), out var cached))
            {
                obj = (T)cached;
                return;
            }

            obj = _container.Resolve<T>();
            if (obj != null)
            {
                Cache[typeof(T)] = obj;
            }
        }

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        private static void ValidateCallingSite<T>()
        {
            var stackTrace = new StackTrace();
            var frame = stackTrace.GetFrame(2); // skip frame 0 (ValidateCallingSite) and frame 1 (Get<T>)
            var method = frame?.GetMethod();
            var declaringType = method?.DeclaringType;
            if (declaringType == null)
                return;

            EnsureNotCalledFrom<MonoBehaviour>();
            EnsureNotCalledFrom<ScriptableObject>();
            return;

            void EnsureNotCalledFrom<K>()
            {
                string msg =
                    $"Do not call ServiceLocator.Get<{typeof(T).Name}>() from {typeof(K).Name} '{declaringType.Name}.{method.Name}()'. Use [Inject] attribute instead.";
                Debug.Assert(!typeof(K).IsAssignableFrom(declaringType), msg);
            }
        }
#endif
    }
    
#if UNITY_EDITOR
    /// <summary>
    /// Prevents stale service instances from being used across Play Mode sessions.
    /// DiContainer is recreated on each Play Mode entry, and objects then hold invalid references to past service instances.
    /// </summary>
    [InitializeOnLoad]
    public class ServiceLocatorInvalidator
    {
        static ServiceLocatorInvalidator()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state is PlayModeStateChange.EnteredPlayMode or PlayModeStateChange.ExitingPlayMode)
            {
                ServiceLocator.ClearCache();
            }
        }
    }
#endif
}