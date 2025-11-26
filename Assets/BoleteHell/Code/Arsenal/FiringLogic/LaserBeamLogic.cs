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
            Collider2D previousHitCollider = null;
            CurrentPos = bulletSpawnPoint;
            _rayPositions.Add(CurrentPos);
            CurrentDirection = direction.normalized;
            
            LaserInstance laserInstance = LaserRendererPool.Instance.Get(instigator, laserCombo.LaserAllegiance);
            bool destroy = false;
            for (int i = 0; i <= cannonData.maxNumberOfBounces; i++)
            {
                LayerMask layerMask = ~LayerMask.GetMask("IgnoreProjectile");
                RaycastHit2D[] hits = Physics2D.RaycastAll(CurrentPos, CurrentDirection, cannonData.maxRayDistance, layerMask);
                
                Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));
                bool hasInteraction = false;

                //Pour chaque hit on fait son éffet
                foreach (RaycastHit2D currentHit in hits)
                {
                    if (previousHitCollider && currentHit.collider == previousHitCollider)
                        continue; 
                    Vector3 hitPos = currentHit.point; //On ajoute un petit offset pour éviter de toucher le collider à nouveau
                    
                    ITargetable.Context context = new(currentHit.collider.gameObject, instigator, laserInstance, currentHit, CurrentDirection, laserCombo);
                    OnHit(context, altered =>
                    {
                        CurrentDirection = altered.Direction;

                        hasInteraction = altered.BlockProjectile || altered.RequestDestroyProjectile;
                        destroy = altered.RequestDestroyProjectile;
                    });

                    if (hasInteraction)
                    {
                        //Empêche de toucher le même collider deux fois de suite
                        //Sert surtout car le refract shield doit refaire le raycast a partir du point toucher mais  comme il va dans la même direction général qu'au début
                        //il retouche toujours le même coté touché précédemment
                        //donc je doit lui permettre de skipper le mur touché et faire le check des prochaines choses
                        _rayPositions.Add(hitPos);
                        CurrentPos = hitPos;
                        previousHitCollider = currentHit.collider;
                        break;
                    }
                }
                
                //Si on a pas d'interactions on force l'arret des rebondissement
                if (destroy || !hasInteraction)
                    break;
            }
            
            //Si le laser ne se fait pas détruire on déssine sa destination final
            if (!destroy)
            {
                Vector2 finalPoint = (Vector2)CurrentPos + CurrentDirection * cannonData.maxRayDistance;
                _rayPositions.Add(finalPoint);
            }

            laserInstance.DrawRay(_rayPositions, laserCombo.CombinedColor, cannonData.Lifetime);

            _rayPositions.Clear();
        }

        public override void FinishFiring()
        {

        }
    }
}
