using System.Collections.Generic;
using BoleteHell.Arsenals.Cannons;
using BoleteHell.Arsenals.HitHandler;
using BoleteHell.Arsenals.LaserData;
using BoleteHell.Arsenals.Rays;
using UnityEngine;

namespace BoleteHell.Arsenals.FiringLogic
{
    public class LaserProjectileLogic : FiringLogic
    {
        public override void Shoot(Vector3 bulletSpawnPoint, Vector2 direction, CannonData data, LaserCombo laserCombo, GameObject instigator = null)
        {
            // crée seulement un point de début et un point de fin
            LaserInstance reservedRenderer = LaserRendererPool.Instance.Get();
            List<Vector3> positions = new List<Vector3> { Vector3.zero, Vector3.up * reservedRenderer.LaserLength };
            reservedRenderer.transform.position = bulletSpawnPoint;
            reservedRenderer.DrawRay(positions, laserCombo.CombinedColor, data.Lifetime);
            LaserProjectileMovement projectileMovement = reservedRenderer.SetupProjectileLaser(direction, laserCombo.GetLaserSpeed());
            projectileMovement.OnCollide += ((hit) =>
            {
                ITargetable.Context context = new(hit.gameObject, instigator, reservedRenderer, projectileMovement.gameObject.transform.position, direction, laserCombo);
                OnHit(context, resp =>
                {
                    projectileMovement.SetDirection(resp.Direction);
                });
            });  
        }
    
        public override void FinishFiring()
        {
        
        }
    }
}
