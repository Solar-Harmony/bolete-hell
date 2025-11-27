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
        public override void Shoot(Vector3 bulletSpawnPoint, Vector2 direction, CannonData data, LaserCombo laserCombo,
            GameObject instigator)
        {
            Vector2 currentDirection = direction;
            // crée seulement un point de début et un point de fin
            LaserInstance reservedRenderer = LaserRendererPool.Instance.Get(instigator, laserCombo.LaserAllegiance);
            List<Vector3> positions = new List<Vector3> { Vector3.zero, Vector3.up * reservedRenderer.LaserLength };
            reservedRenderer.transform.position = bulletSpawnPoint;
            reservedRenderer.DrawRay(positions, laserCombo.CombinedColor, data.Lifetime);
            LaserProjectileMovement projectileMovement = reservedRenderer.SetupProjectileLaser(currentDirection, laserCombo.GetLaserSpeed());
            projectileMovement.OnCollide += (hit) =>
            {
                LayerMask layerMask = ~LayerMask.GetMask("IgnoreProjectile");
                RaycastHit2D rayHit = Physics2D.Raycast(projectileMovement.gameObject.transform.position,
                    currentDirection, Mathf.Infinity, layerMask); 
                ITargetable.Context context = new(hit.gameObject, instigator, reservedRenderer, rayHit, currentDirection, laserCombo);
                OnHit(context, resp =>
                {
                    currentDirection = resp.Direction;
                    projectileMovement.SetDirection(resp.Direction);
                });
            };  
        }
    
        public override void FinishFiring()
        {
        
        }
    }
}
