using BulletHell.Scripts.Lines;
using UnityEngine;

public class MirrorLogic:LineHitLogic
{
    public void ExecuteRay(Vector3 incomingDirection, RaycastHit hitPoint,Ray ray)
    {
        Vector3 direction = Vector3.Reflect(incomingDirection, hitPoint.normal);
        //Le float permet de ne pas recast le même point infiniment (desfois selon l'angle ça explosait de rayons)
        ray.Cast(hitPoint.point + direction * 0.001f,direction);
    }

    public void ExecuteProjectile(Vector3 incomingDirection)
    {
        throw new System.NotImplementedException();
    }

   
}
