using System;
using UnityEngine;

namespace BoleteHell.Graphics
{
    public class DeleteLater : MonoBehaviour
    {
        public float lifetime = 0.5f;
        
        private void Start()
        {
            Destroy(gameObject, lifetime);
        }
    }
}