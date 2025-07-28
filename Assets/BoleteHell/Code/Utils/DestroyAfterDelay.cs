using System;
using UnityEngine;

namespace BoleteHell.Code.Utils
{
    [Obsolete("Nique ta mère")]
    public class DestroyAfterDelay : MonoBehaviour
    {
        public float lifetime = 0.5f;

        private void Start()
        {
            Destroy(gameObject, lifetime);
        }
    }
}