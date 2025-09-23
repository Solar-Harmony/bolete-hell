using System.Collections.Generic;
using BoleteHell.Code.Arsenal.Cannons;
using BoleteHell.Code.Arsenal.HitHandler;
using BoleteHell.Code.Arsenal.RayData;
using BoleteHell.Code.Arsenal.Rays;
using BoleteHell.Code.Gameplay.Character;
using UnityEngine;

namespace BoleteHell.Code.Arsenal.FiringLogic
{
    public class LaserBeamLogic : FiringLogic
    {
        private readonly List<Vector3> _rayPositions = new();
        //Modifier pour ne plus réserver un renderer et le réutiliser car ca ne fonctionne pas avec le tire de multiple laser en même temps malheureusement
        //on a une seule instance de LaserBeamLogic donc si l'instance réserve un renderer le même va être utiliser pour tout les tires
        //Serais possible si on informe le LaserBeamLogic du nombre de renderer a réserver 
        public override void Shoot(Vector3 bulletSpawnPoint, Vector2 direction, CannonData cannonData, LaserCombo laserCombo, IFaction instigator)
        {
            Cast(bulletSpawnPoint, direction, cannonData, laserCombo, instigator);
        }

        public override void FinishFiring()
        {

        }
     
        private void Cast(Vector3 bulletSpawnPoint, Vector2 direction, CannonData cannonData, LaserCombo laserCombo, IFaction instigator)
        {
            CurrentPos = bulletSpawnPoint;
            _rayPositions.Add(CurrentPos);
            CurrentDirection = direction;
            LaserInstance laserInstance = LaserRendererPool.Instance.Get();
            
            for (int i = 0; i <= cannonData.maxNumberOfBounces; i++)
            {
                LayerMask layerMask = ~LayerMask.GetMask("IgnoreProjectile");

                RaycastHit2D hit = Physics2D.Raycast(CurrentPos, CurrentDirection, cannonData.maxRayDistance, layerMask);
                
                if (!hit)
                {
                    _rayPositions.Add((Vector2)CurrentPos + CurrentDirection * cannonData.maxRayDistance);
                    break;
                }

                bool shouldBreak = false;
                CurrentPos = hit.point - CurrentDirection * 0.01f; //On ajoute un petit offset pour éviter de toucher le collider à nouveau

                ITargetable.Context context = new(hit.collider.gameObject, instigator, laserInstance, CurrentPos, CurrentDirection, laserCombo);
                OnHit(context, altered =>
                {
                    CurrentDirection = altered.Direction;
                    _rayPositions.Add(CurrentPos);

                    if (altered.RequestDestroyProjectile)
                    {
                        shouldBreak = true;
                    }
                });

                if (shouldBreak)
                    break;
            }
            laserInstance.DrawRay(_rayPositions, laserCombo.CombinedColor, cannonData.Lifetime, instigator);

            _rayPositions.Clear();
        }
    }
}
