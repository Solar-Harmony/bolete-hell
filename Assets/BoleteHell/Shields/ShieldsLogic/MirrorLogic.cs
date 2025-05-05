using System;
using UnityEngine;

namespace Shields.ShieldsLogic
{
    [Serializable]
    public class MirrorLogic : ILineHitLogic
    {
        public Vector3 ExecuteRay(Vector3 incomingDirection, RaycastHit2D hitPoint, float lightRefractiveIndice)
        {
            return Vector2.Reflect(incomingDirection, hitPoint.normal);
        }

        public void ExecuteProjectile(Vector3 incomingDirection)
        {
            throw new NotImplementedException();
        }
    }
}