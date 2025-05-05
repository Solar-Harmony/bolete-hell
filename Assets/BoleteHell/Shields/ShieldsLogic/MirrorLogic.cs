using System;
using BulletHell.Scripts.Lines;
using UnityEngine;


[Serializable]
public class MirrorLogic:LineHitLogic
{
    public Vector3 ExecuteRay(Vector3 incomingDirection, RaycastHit2D hitPoint,float lightRefractiveIndice)
    {
        return Vector2.Reflect(incomingDirection, hitPoint.normal);
    }

    public void ExecuteProjectile(Vector3 incomingDirection)
    {
        throw new System.NotImplementedException();
    }

   
}
