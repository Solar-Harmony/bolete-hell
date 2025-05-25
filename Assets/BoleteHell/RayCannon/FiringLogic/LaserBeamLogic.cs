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

    public override void OnReset(LaserRenderer renderer)
    {
        renderer.gameObject.SetActive(false);
    }

    public override void Shoot(Vector3 bulletSpawnPoint, Vector2 direction,RayCannonData rayCannonData,CombinedLaser laser)
    {

        if (!_reservedRenderer)
        {
            _reservedRenderer = LineRendererPool.Instance.Get();
            UpdateChargeTime(rayCannonData.timeBetweenShots);
        }

        //Debug.Log($"next shot in {Time.time - NextShootTime}");
        if (!(Time.time >= NextShootTime)) return;
        Cast(bulletSpawnPoint, direction,rayCannonData,laser);
        UpdateChargeTime(rayCannonData.timeBetweenShots);
    }

    public override void FinishFiring()
    {
        if (!_reservedRenderer) return;
        
        LineRendererPool.Instance.Release(_reservedRenderer);
        _reservedRenderer = null;

    }
    
    private void Cast(Vector3 bulletSpawnPoint, Vector2 direction,RayCannonData rayCannonData,CombinedLaser laser)
    {
        CurrentPos = bulletSpawnPoint;
        _rayPositions.Add(CurrentPos);
        CurrentDirection = direction;

        for (int i = 0; i <= rayCannonData.maxNumberOfBounces; i++)
        {
            RaycastHit2D hit = Physics2D.Raycast(CurrentPos, CurrentDirection,rayCannonData.maxRayDistance);
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
        
        _reservedRenderer.DrawRay(_rayPositions,laser.CombinedColor,rayCannonData.lifeTime,this);
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

    private void UpdateChargeTime(float timeBetweenShots)
    {
        NextShootTime = Time.time + timeBetweenShots;
    }
    
}
