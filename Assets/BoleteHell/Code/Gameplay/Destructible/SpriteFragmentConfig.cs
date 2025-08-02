using System;
using UnityEngine;

namespace BoleteHell.Code.Gameplay.Destructible
{
    [Serializable]
    public class SpriteFragmentConfig
    {
        [Header("General config")]
        [field: SerializeField] 
        public SpriteRenderer sr { get; private set; }
        
        [field: SerializeField] 
        public Rigidbody2D parentRb { get; private set; }
        
        [field: SerializeField] 
        public GameObject explosion { get; private set; }

        [Header("Explosion settings")]
        [field: SerializeField] 
        public int fragmentsX { get; private set; } = 4;

        [field: SerializeField]
        public int fragmentsY { get; private set; } = 4;

        [field: SerializeField] 
        public float explosionForce { get; private set; } = 1.0f;
    }
}