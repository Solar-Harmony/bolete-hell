using System;
using UnityEngine;

namespace Shields.ShieldsLogic
{
    [Serializable]
    public class RefractLogic : ILineHitLogic
    {
        [SerializeField] private float materialRefractiveIndice = 2.417f;

        public Vector3 ExecuteRay(Vector3 incomingDirection, RaycastHit2D hitPoint, float lightRefractiveIndice)
        {
            return Refract(incomingDirection, hitPoint.normal, lightRefractiveIndice,
                materialRefractiveIndice);
        }

        public void ExecuteProjectile(Vector3 incomingDirection)
        {
            throw new NotImplementedException();
        }

        private Vector2 Refract(Vector3 incidentDirection, Vector3 surfaceNormal, float refractiveIndex1,
            float refractiveIndex2)
        {
            var changeScale = refractiveIndex1 / refractiveIndex2;
            var cosI = -Vector3.Dot(surfaceNormal, incidentDirection);
            var sinT2 = changeScale * changeScale * (1 - cosI * cosI);

            if (sinT2 > 1)
                return Vector2.zero;

            var cosT = Mathf.Sqrt(1 - sinT2);
            return changeScale * incidentDirection + (changeScale * cosI - cosT) * surfaceNormal;
        }
    }
}