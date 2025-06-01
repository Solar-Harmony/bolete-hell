using System;
using System.Collections.Generic;
using BoleteHell.Rays;
using BoleteHell.Utils;
using Data.Cannons;
using Data.Rays;
using Lasers;
using Shields;
using UnityEngine;

public class LaserBeamLogic:RayCannonFiringLogic
{
    private LaserRenderer _reservedRenderer;
    private readonly List<Vector3> _rayPositions = new();
    //Modifier pour ne plus réserver un renderer et le réutiliser car ca ne fonctionne pas avec le tire de multiple laser en même temps malheureusement
    //on a une seule instance de LaserBeamLogic donc si l'instance réserve un renderer le même va être utiliser pour tout les tires
    //Serais possible si on informe le LaserBeamLogic du nombre de renderer a réserver 
    public override void OnReset(LaserRenderer renderer)
    {
        LineRendererPool.Instance.Release(renderer);
        _reservedRenderer = null;
    }

    public override void Shoot(Vector3 bulletSpawnPoint, Vector2 direction,RayCannonData rayCannonData,CombinedLaser laser)
    {
        _reservedRenderer = LineRendererPool.Instance.Get();
        
        Cast(bulletSpawnPoint, direction,rayCannonData,laser);
    }

    public override void FinishFiring()
    {

    }
    
    private void Cast(Vector3 bulletSpawnPoint, Vector2 direction,RayCannonData rayCannonData,CombinedLaser laser)
    {
        CurrentPos = bulletSpawnPoint;
        _rayPositions.Add(CurrentPos);
        CurrentDirection = direction;

        for (int i = 0; i <= rayCannonData.maxNumberOfBounces; i++)
        {
            LayerMask layerMask = ~LayerMask.GetMask("Projectile");

            RaycastHit2D hit = Physics2D.Raycast(CurrentPos, CurrentDirection,rayCannonData.maxRayDistance,layerMask);
            if (!hit)
            {
                //Debug.DrawRay(CurrentPos, CurrentDirection * _laserData.MaxRayDistance, Color.black);
                _rayPositions.Add((Vector2)CurrentPos + CurrentDirection * rayCannonData.maxRayDistance);
                break;
            }

            if (hit.transform.gameObject.TryGetComponent(out Shield lineHit))
            {
                OnHitShield(hit, lineHit,laser.CombinedRefractiveIndex);
            }
            //Devrait check le health component de la personne pour que ça fonctionne si on touche un ennemi ou le joueur
            else if (hit.transform.gameObject.TryGetComponent(out Health health))
            {
                OnHitEnemy(hit.point,health,laser);
                //Si je touche un ennemi je ne refait plus de bounces
                break;
            }
            else 
            {
                _rayPositions.Add(hit.point); 
                break;
            }
        }
        
        _reservedRenderer.DrawRay(_rayPositions,laser.CombinedColor,rayCannonData.LifeTime,this);
        _rayPositions.Clear();
    }

    private void OnHitEnemy(Vector2 hitPosition,Health health,CombinedLaser laser)
    {
        _rayPositions.Add(hitPosition);
        laser.CombinedEffect(hitPosition,health);
    }

    private void OnHitShield(RaycastHit2D hitPoint, Shield shieldHit,float lightRefractiveIndex)
    {
        Debug.DrawLine(CurrentPos, hitPoint.point, Color.blue);
        CurrentDirection = shieldHit.OnRayHitLine(CurrentDirection, hitPoint, lightRefractiveIndex);
        CurrentPos = hitPoint.point + CurrentDirection * 0.01f;
        _rayPositions.Add(CurrentPos);
    }

}
