using System;
using UnityEngine;

namespace BoleteHell.Code.Arsenal.Shields.ShieldsLogic
{
    [Serializable]
    public class MirrorLogic : IShieldHitLogic
    {
        public bool ShouldBlocklaser { get; set; } = true;

        public Vector2 ExecuteRay(Vector3 incomingDirection, RaycastHit2D hitPoint, float lightRefractiveIndice)
        {
            return Vector2.Reflect(incomingDirection, hitPoint.normal);
        }
    }
}