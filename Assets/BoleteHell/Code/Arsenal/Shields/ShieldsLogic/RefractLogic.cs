using System;
using UnityEngine;

namespace BoleteHell.Code.Arsenal.Shields.ShieldsLogic
{
    [Serializable]
    public class RefractLogic : IShieldHitLogic
    {
        [SerializeField] private float materialRefractiveIndice = 2.417f;

        [field:SerializeField]public bool ShouldBlocklaser { get; set; } = true;

        public Vector2 ExecuteRay(Vector3 incomingDirection, RaycastHit2D hitPoint, float lightRefractiveIndice)
        {
            return Refract(incomingDirection, hitPoint.normal, lightRefractiveIndice, materialRefractiveIndice);
        }

        private Vector2 Refract(Vector2 incidentDirection, Vector2 surfaceNormal, float refractiveIndex1,
            float refractiveIndex2)
        {
            var changeScale = refractiveIndex1 / refractiveIndex2;
            var cosI = -Vector2.Dot(surfaceNormal, incidentDirection);
            var sinT2 = changeScale * changeScale * (1 - cosI * cosI);

            if (sinT2 > 1)
                return Vector2.zero;

            var cosT = Mathf.Sqrt(1 - sinT2);
            return changeScale * incidentDirection + (changeScale * cosI - cosT) * surfaceNormal;
        }
    }
}