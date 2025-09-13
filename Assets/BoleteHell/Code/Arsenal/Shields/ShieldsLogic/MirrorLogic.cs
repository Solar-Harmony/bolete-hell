using System;
using UnityEngine;

namespace BoleteHell.Code.Arsenal.Shields.ShieldsLogic
{
    [Serializable]
    public class MirrorLogic : IShieldHitLogic
    {
        public Vector2 ExecuteRay(Vector3 incomingDirection, RaycastHit2D hitPoint, float lightRefractiveIndice)
        {
            Debug.Log($"incoming direction: {incomingDirection}, normal: {hitPoint.normal}" );

            return Vector2.Reflect(incomingDirection, hitPoint.normal);
        }
    }
}