using System.Collections.Generic;
using BoleteHell.Code.Arsenal.Cannons;
using BoleteHell.Code.Arsenal.HitHandler;
using BoleteHell.Code.Arsenal.RayData;
using BoleteHell.Code.Arsenal.Rays;
using BoleteHell.Code.Gameplay.Characters;
using UnityEngine;

namespace BoleteHell.Code.Arsenal.FiringLogic
{
    public class LaserProjectileLogic : FiringLogic
    {
        public override void Shoot(Vector3 bulletSpawnPoint, Vector2 direction, CannonData data, LaserCombo laserCombo,
            IInstigator instigator)
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
                ITargetable.Context context = new(hit.gameObject, instigator, reservedRenderer, projectileMovement.gameObject.transform.position, currentDirection, laserCombo);
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
