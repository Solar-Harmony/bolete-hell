using System;
using UnityEngine;

namespace BulletHell.Scripts.Lines
{
    public interface LineHitLogic
    {
        public Vector3 ExecuteRay(Vector3 incomingDirection,RaycastHit2D hitPoint,float lightRefractiveIndice);
        
        public void ExecuteProjectile(Vector3 incomingDirection);
    }
}
