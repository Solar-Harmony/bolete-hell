using System.Collections.Generic;
using BoleteHell.Code.Arsenal.Cannons;
using BoleteHell.Code.Arsenal.HitHandler;
using BoleteHell.Code.Arsenal.RayData;
using BoleteHell.Code.Arsenal.Rays;
using UnityEngine;

namespace BoleteHell.Code.Arsenal.FiringLogic
{
    public class LaserProjectileLogic : FiringLogic
    {
        public override void Shoot(Vector3 bulletSpawnPoint, Vector2 direction, CannonData data, LaserCombo laserCombo, GameObject instigator = null)
        {
            // crée seulement un point de début et un point de fin
            LaserRenderer reservedRenderer = LaserRendererPool.Instance.Get();
            List<Vector3> positions = new List<Vector3> { Vector3.zero, Vector3.up * reservedRenderer.LaserLength };
            reservedRenderer.transform.position = bulletSpawnPoint;
            reservedRenderer.DrawRay(positions, laserCombo.CombinedColor, data.LifeTime,this);
            var projectile = reservedRenderer.SetupProjectileLaser(direction, data.projectileSpeed);
            projectile.OnCollide +=  (hit) =>
            {
                IHitHandler.Context context = new(hit.gameObject, instigator, projectile.gameObject, projectile.gameObject.transform.position, direction, laserCombo);
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
}
