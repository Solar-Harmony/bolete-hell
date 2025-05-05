using System;
using UnityEngine;

namespace Lasers.RayLogic
{
    [Serializable]
    public abstract class RayHitLogic
    {
        [SerializeField] public float baseDamage = 10f;
        [SerializeField] public float projectileSpeed = 20f;
        public abstract void OnHit();
    }
}