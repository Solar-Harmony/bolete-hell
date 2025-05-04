using BulletHell.Scripts.Lines;
using UnityEngine;

public class MirrorLogic:LineHitLogic
{
    public void ExecuteRay(Vector3 incomingDirection, RaycastHit2D hitPoint,Ray ray)
    {
        Vector2 direction = Vector2.Reflect(incomingDirection, hitPoint.normal);
        //Le float permet de ne pas recast le même point infiniment (desfois selon l'angle ça explosait de rayons)
        ray.Cast(hitPoint.point + direction * 0.1f,direction);
    }

    public void ExecuteProjectile(Vector3 incomingDirection)
    {
        throw new System.NotImplementedException();
    }

   
}
