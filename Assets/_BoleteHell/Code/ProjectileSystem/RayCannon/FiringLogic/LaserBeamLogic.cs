using System;
using System.Collections.Generic;
using _BoleteHell.Code.Character;
using _BoleteHell.Code.ProjectileSystem.HitHandler;
using BoleteHell.Rays;
using BoleteHell.Utils;
using Data.Cannons;
using Data.Rays;
using Lasers;
using Shields;
using UnityEngine;

public class LaserBeamLogic : FiringLogic
{
    private readonly List<Vector3> _rayPositions = new();
    //Modifier pour ne plus réserver un renderer et le réutiliser car ca ne fonctionne pas avec le tire de multiple laser en même temps malheureusement
    //on a une seule instance de LaserBeamLogic donc si l'instance réserve un renderer le même va être utiliser pour tout les tires
    //Serais possible si on informe le LaserBeamLogic du nombre de renderer a réserver 
    public override void Shoot(Vector3 bulletSpawnPoint, Vector2 direction, RayCannonData rayCannonData, CombinedLaser laser, GameObject instigator = null)
    {
        Cast(bulletSpawnPoint, direction,rayCannonData,laser);
    }

    public override void FinishFiring()
    {

    }
     
    private void Cast(Vector3 bulletSpawnPoint, Vector2 direction, RayCannonData rayCannonData, CombinedLaser laser)
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
                _rayPositions.Add((Vector2)CurrentPos + CurrentDirection * rayCannonData.maxRayDistance);
                break;
            }
            
            IHitHandler.Context context = new(hit.collider.gameObject, null, null, CurrentPos, CurrentDirection, laser);
            IHitHandler.TryHandleHit(context, altered =>
            {
                CurrentDirection = altered.Direction;
                CurrentPos = hit.point + CurrentDirection * 0.01f; //On ajoute un petit offset pour éviter de toucher le collider à nouveau
                _rayPositions.Add(CurrentPos);
            });
        }
        
        LaserRendererPool.Instance.Get().DrawRay(_rayPositions,laser.CombinedColor,rayCannonData.LifeTime,this);
        _rayPositions.Clear();
    }
}
