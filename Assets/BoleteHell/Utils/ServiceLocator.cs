using UnityEngine;
using Zenject;

namespace BoleteHell.Utils
{
    public static class ServiceLocator
    {
        private static DiContainer _container = null;
        public static void Initialize(DiContainer container)
        {
            _container ??= container;
        }
        
        // Injects dependencies into the given object.
        // This is a WORKAROUND, only use it for objects that cannot be injected otherwise.
        // Whose lifetime is managed outside of our code and opaque.
        public static void Inject(object obj)
        {
            Debug.Assert(_container != null);
            _container.Inject(obj);
        }
    }
}