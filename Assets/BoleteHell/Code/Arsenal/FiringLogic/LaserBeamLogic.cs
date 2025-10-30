using System;
using System.Collections.Generic;
using BoleteHell.Code.Arsenal.Cannons;
using BoleteHell.Code.Arsenal.HitHandler;
using BoleteHell.Code.Arsenal.RayData;
using BoleteHell.Code.Arsenal.Rays;
using BoleteHell.Code.Gameplay.Characters;
using UnityEngine;

namespace BoleteHell.Code.Arsenal.FiringLogic
{
    public class LaserBeamLogic : FiringLogic
    {
        private readonly List<Vector3> _rayPositions = new();
        
        public override void Shoot(Vector3 bulletSpawnPoint, Vector2 direction, CannonData cannonData, LaserCombo laserCombo, IInstigator instigator)
        {
            CurrentPos = bulletSpawnPoint;
            _rayPositions.Add(CurrentPos);
            CurrentDirection = direction;
            LaserInstance laserInstance = LaserRendererPool.Instance.Get(instigator, laserCombo.LaserAllegiance);
            
            for (int i = 0; i <= cannonData.maxNumberOfBounces; i++)
            {
                LayerMask layerMask = ~LayerMask.GetMask("IgnoreProjectile");
                bool destroy = false;
                RaycastHit2D[] realHits = Physics2D.RaycastAll(CurrentPos, CurrentDirection, cannonData.maxRayDistance, layerMask);
                //créé le tableau avec une taille de +1 pour le fake hit ajouté
                RaycastHit2D[] hits = new RaycastHit2D[realHits.Length + 1];
                Array.Copy(realHits, hits, realHits.Length);

                //Si rien ne bloque ou détruit un laser il devrait se rendre au maximum de son range donc j'ajoute un fake hit au point final du laser
                RaycastHit2D lastPoint = new RaycastHit2D
                {
                    point = (Vector2)CurrentPos + CurrentDirection * cannonData.maxRayDistance,
                    distance = cannonData.maxRayDistance
                };
                
                hits[^1] = lastPoint;

                
                Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));
                //Pour chaque hit on fait son éffet
                foreach (RaycastHit2D currentHit in hits)
                {
                    bool blocked = false;
                    CurrentPos = currentHit.point - CurrentDirection * 0.01f; //On ajoute un petit offset pour éviter de toucher le collider à nouveau

                    if (currentHit == lastPoint)
                    {
                        _rayPositions.Add(CurrentPos);
                        destroy = true;
                        break;
                    }
                    
                    ITargetable.Context context = new(currentHit.collider.gameObject, instigator, laserInstance, CurrentPos, CurrentDirection, laserCombo);
                    OnHit(context, altered =>
                    {
                        CurrentDirection = altered.Direction;
                        _rayPositions.Add(CurrentPos);
                        
                        blocked = altered.BlockProjectile || altered.RequestDestroyProjectile;
                        destroy = altered.RequestDestroyProjectile;
                    });

                    if (blocked)
                        break;
                }
                if (destroy)
                    break;
            }
            laserInstance.DrawRay(_rayPositions, laserCombo.CombinedColor, cannonData.Lifetime);

            _rayPositions.Clear();
        }

        public override void FinishFiring()
        {

        }
    }
}
