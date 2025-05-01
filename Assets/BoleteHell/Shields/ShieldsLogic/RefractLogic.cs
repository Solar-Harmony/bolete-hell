using BulletHell.Scripts.Lines;
using UnityEngine;

public class RefractLogic:LineHitLogic
{
    private float materialRefractiveIndice = 2f;
    
    public void ExecuteRay(Vector3 incomingDirection, RaycastHit2D hitPoint,Ray ray)
    {
        Vector2 direction = Refract(incomingDirection, hitPoint.normal, ray.lightRefractiveIndice,
            materialRefractiveIndice);
        Vector3 originPoint = hitPoint.point + direction * 0.001f;
        ray.Cast(originPoint,direction);
    }

    public void ExecuteProjectile(Vector3 incomingDirection)
    {
        throw new System.NotImplementedException();
    }
    
    private Vector2 Refract(Vector3 incidentDirection, Vector3 surfaceNormal, float refractiveIndice1,
        float refractiveIndice2)
    {
        
        float changeScale = refractiveIndice1 / refractiveIndice2;
        float cosI = -Vector3.Dot(surfaceNormal, incidentDirection);
        float sinT2 = changeScale * changeScale * (1 - cosI * cosI);

        if (sinT2 > 1)
            return Vector2.zero;

        float cosT = Mathf.Sqrt(1 - sinT2);
        return changeScale * incidentDirection + (changeScale * cosI - cosT) * surfaceNormal;
    }
}
