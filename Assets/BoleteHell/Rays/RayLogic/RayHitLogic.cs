using System;
using UnityEngine;

namespace Lasers.RayLogic
{
    [Serializable]
    public abstract class RayHitLogic
    {
        public abstract void OnHit(Vector2 hitPosition,Health hitCharacterHealth);
    }
}