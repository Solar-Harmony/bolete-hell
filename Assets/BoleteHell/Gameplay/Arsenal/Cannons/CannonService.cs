using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BoleteHell.Code.Arsenal.ShotPatterns;
using BoleteHell.Utils;
using UnityEngine;
using UnityEngine.UIElements;
using Zenject;

namespace BoleteHell.Code.Arsenal.Cannons
{
    public record ShotLaunchParams(Vector2 CenterPos, Vector2 SpawnPosition, Vector2 SpawnDirection, GameObject Instigator);

    public class CannonService : ICannonService
    {
        [Inject]
        private ICoroutineProvider _coroutine;
        
        [Inject]
        private IShotPatternService _patternService;

        [Inject]
        private LaserPreviewRenderer.Pool _pool;
        
        public void Tick(CannonInstance cannon)
        {
            if (cannon.CanShoot) 
                return;
            
            float currentCooldown = cannon.Config.cannonData.rampsUpFiringSpeed ? 
                cannon.Config.cannonData.GetRampedUpCooldown(cannon.ShotCount) : 
                cannon.Config.cannonData.cooldown;
            
            if (cannon.AttackTimer < currentCooldown)
            {
                cannon.AttackTimer += Time.deltaTime;
            }
            else
            {
                cannon.CanShoot = true;
            }
        }

        public bool TryShoot(CannonInstance cannon, ShotLaunchParams parameters)
        {
            if (!cannon.CanShoot)
                return false;
            
            List<ShotLaunchParams> setupProjectiles = new List<ShotLaunchParams>();
            foreach (ShotPatternData patternData in cannon.Config.GetBulletPatterns())
            {
                setupProjectiles.AddRange(SetupProjectiles(cannon, parameters, patternData));
            }

            if (!cannon.IsCharged)
                return false;
            
            Fire(cannon, setupProjectiles, parameters.Instigator);
            return true;
        }

        //Pourrais peut-etre avoir un bool sur les shotparams qui détermine si le projectile est pret a etre tirer
        //Permettrais d'avoir des pattern qui tire des beam et des projectile en même temps sans faire attendre les projectile car ils ne charge pas
        //Pas nécéssaire pour le moment car les patterns sont spécifique a un firing type
        private List<ShotLaunchParams> SetupProjectiles(CannonInstance cannon, ShotLaunchParams parameters, ShotPatternData patternData)
        {
            List<ShotLaunchParams> projectiles =
                _patternService.ComputeSpawnPoints(patternData, parameters, cannon.ShotCount);

            if (cannon.Config.cannonData.WaitBeforeFiring && !cannon.IsCharged)
            {
                ChargeShot(cannon, projectiles);
            }

            return projectiles;
        }

        private Vector2 GetBeamPreviewEndPoint(CannonInstance cannon, ShotLaunchParams parameters)
        {
            int obstacleLayerMask = LayerMask.GetMask("Obstacle");

            RaycastHit2D hit = Physics2D.Raycast(
                parameters.SpawnPosition,
                parameters.SpawnDirection,
                cannon.Config.cannonData.maxRayDistance,
                obstacleLayerMask
            );
            
            return hit ? hit.point : parameters.SpawnPosition + parameters.SpawnDirection * cannon.Config.cannonData.maxRayDistance;
        }


        private static void SetupNextShot(CannonInstance cannon)
        {
            cannon.ShotCount++;
            cannon.CanShoot = false;
            cannon.IsCharged = !cannon.Config.cannonData.WaitBeforeFiring;
            cannon.AttackTimer = 0f;
            cannon.ChargeTimer = 0f;
        }

        private void ChargeShot(CannonInstance cannon, List<ShotLaunchParams> projectiles)
        {
            if(cannon.reservedPreviewRenderers.Count == 0)
                CreateRenderers(cannon, projectiles);

            for (int i = projectiles.Count - 1 ; i >= 0; i--)
            {
                ShotLaunchParams parameters = projectiles[i];

                if (cannon.ChargeTimer < cannon.Config.cannonData.chargeTime)
                {
                    cannon.reservedPreviewRenderers[i].UpdatePreview(
                        parameters.SpawnPosition,
                        GetBeamPreviewEndPoint(cannon, parameters),
                        cannon.ChargeTimer);
                }
                else
                {
                    cannon.reservedPreviewRenderers[i].Despawn();
                    cannon.reservedPreviewRenderers.RemoveAt(i);
                    cannon.IsCharged = true;
                }
            }
            
            cannon.ChargeTimer += Time.deltaTime;
        }

        private void CreateRenderers(CannonInstance cannon, List<ShotLaunchParams> projectiles)
        {
            foreach (ShotLaunchParams projectile in projectiles)
            {
                cannon.reservedPreviewRenderers.Add(_pool.Spawn(
                    projectile.SpawnPosition,
                    GetBeamPreviewEndPoint(cannon, projectile),
                    cannon.LaserCombo.CombinedColor,
                    cannon.Config.cannonData.chargeTime));
            }
        }

        private void Fire(CannonInstance cannon, List<ShotLaunchParams> projectileLaunchData, GameObject instigator)
        {
            foreach (ShotLaunchParams launchData in projectileLaunchData)
            {
                cannon.CurrentFiringLogic?.Shoot(launchData.SpawnPosition, launchData.SpawnDirection, cannon.Config.cannonData, cannon.LaserCombo, instigator);
            }

            SetupNextShot(cannon);
        }

        public void FinishFiring(CannonInstance cannon)
        {
            foreach (LaserPreviewRenderer reservedPreviewRenderer in cannon.reservedPreviewRenderers.ToList())
            {
                if(!reservedPreviewRenderer) continue;
                
                reservedPreviewRenderer.Despawn();
                
                cannon.reservedPreviewRenderers.Remove(reservedPreviewRenderer);
            }
            cannon.ChargeTimer = 0;
            cannon.CurrentFiringLogic?.FinishFiring();
        }
    }
}