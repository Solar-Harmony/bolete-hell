using System.Diagnostics;
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

        internal static void Initialize(DiContainer container)
        {
            _container = container;
        }

        public static void Get<T>(out T obj)
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            ValidateCallingSite<T>();
#endif

            obj = _container.Resolve<T>();
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
                    $"Do not call ServiceLocator.Get<{typeof(T).Name}>() in {typeof(K).Name} '{declaringType.Name}.{method.Name}()'. Use [Inject] attribute instead.";
                Debug.Assert(!typeof(K).IsAssignableFrom(declaringType), msg);
            }
        }
#endif
    }
}