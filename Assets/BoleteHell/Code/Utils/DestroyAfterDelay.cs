using UnityEngine;

namespace BoleteHell.Code.Utils
{
    public class DestroyAfterDelay : MonoBehaviour
    {
        public float lifetime = 0.5f;

        private void Start()
        {
            Destroy(gameObject, lifetime);
        }
    }
}