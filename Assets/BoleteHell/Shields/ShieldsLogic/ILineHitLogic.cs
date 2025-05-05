using UnityEngine;

namespace Shields.ShieldsLogic
{
    public interface ILineHitLogic
    {
        public Vector3 ExecuteRay(Vector3 incomingDirection, RaycastHit2D hitPoint, float lightRefractiveIndice);
        public void ExecuteProjectile(Vector3 incomingDirection);
    }
}