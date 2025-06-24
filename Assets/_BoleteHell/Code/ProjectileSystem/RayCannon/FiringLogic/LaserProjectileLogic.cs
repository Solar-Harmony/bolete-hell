using System;
using System.Collections.Generic;
using _BoleteHell.Code.ProjectileSystem.HitHandler;
using BoleteHell.Rays;
using BoleteHell.Utils;
using Data.Cannons;
using Data.Rays;
using Lasers;
using Shields;
using UnityEngine;


public class LaserProjectileLogic : FiringLogic
{
    public override void Shoot(Vector3 bulletSpawnPoint, Vector2 direction, RayCannonData data, CombinedLaser laser, GameObject instigator = null)
    {
        // crée seulement un point de début et un point de fin
        LaserRenderer reservedRenderer = LaserRendererPool.Instance.Get();
        List<Vector3> positions = new List<Vector3> { Vector3.zero, Vector3.up * reservedRenderer.LaserLength };
        reservedRenderer.transform.position = bulletSpawnPoint;
        reservedRenderer.DrawRay(positions, laser.CombinedColor, data.LifeTime,this);
        var projectile = reservedRenderer.SetupProjectileLaser(direction, data.projectileSpeed);
        projectile.OnCollide +=  (hit) =>
        {
            IHitHandler.Context context = new(hit.gameObject, instigator, projectile.gameObject,reservedRenderer, projectile.gameObject.transform.position, direction, laser);
            OnHit(context, resp =>
            {
                projectile.SetDirection(resp.Direction);
            });
        }; 
    }
    
    public override void FinishFiring()
    {
        
    }
}
