using System;
using Shields.ShieldsLogic;
using UnityEngine;

namespace BoleteHell.Shields.ShieldsLogic
{
    [Serializable]
    public class MirrorLogic : IShieldHitLogic
    {
        public Vector2 ExecuteRay(Vector3 incomingDirection, RaycastHit2D hitPoint, float lightRefractiveIndice)
        {
            Debug.Log($"incoming direction: {incomingDirection}, normal: {hitPoint.normal}" );

            return Vector2.Reflect(incomingDirection, hitPoint.normal);
        }

        public Vector3 ExecuteProjectile(Vector3 incomingDirection)
        {
            throw new NotImplementedException();
        }
    }
}