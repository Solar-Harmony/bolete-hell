using System;
using UnityEngine;

namespace Shields.ShieldsLogic
{
    public interface IShieldHitLogic
    {
        public Vector3 ExecuteRay(Vector3 incomingDirection, RaycastHit2D hitPoint, float lightRefractiveIndice);
        public Vector3 ExecuteProjectile(Vector3 incomingDirection);
    }
}