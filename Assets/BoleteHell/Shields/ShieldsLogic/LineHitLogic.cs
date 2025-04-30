using UnityEngine;

namespace BulletHell.Scripts.Lines
{
    
    public interface LineHitLogic
    {
        public void ExecuteRay(Vector3 incomingDirection,RaycastHit hitPoint,Ray ray);
        
        public void ExecuteProjectile(Vector3 incomingDirection);
    }
}
