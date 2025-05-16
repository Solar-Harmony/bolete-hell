using System;
using UnityEngine;

namespace Lasers.RayLogic
{
    [Serializable]
    public abstract class RayHitLogic
    {
        [SerializeField] public float baseDamage = 10f;
        public abstract void OnHit();
    }
}