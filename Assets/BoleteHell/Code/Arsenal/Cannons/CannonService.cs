using System.Collections;
using System.Collections.Generic;
using BoleteHell.Code.Arsenal.ShotPatterns;
using BoleteHell.Code.Utils;
using UnityEngine;
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

        private LaserPreviewRenderer beamPreview;
        public void Tick(CannonInstance cannon)
        {
            if (cannon.CanShoot) 
                return;
            
            if (cannon.AttackTimer < cannon.Config.cannonData.cooldown)
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
            if (!cannon.CanShoot) return false;
            
            if (cannon.Config.cannonData.WaitBeforeFiring && !cannon.IsCharged)
            {
                if (!beamPreview)
                {
                    //Setup du preview
                    //Le preview ne montre pas les réflections et refractions etc
                    beamPreview = _pool.Spawn(parameters.SpawnPosition, GetBeamPreviewEndPoint(cannon, parameters), cannon.LaserCombo.CombinedColor,
                        cannon.Config.cannonData.chargeTime);
                }

                ChargeShot(cannon, parameters);
                return false;
            }
            
            FireProjectiles(cannon, parameters);
            return true;
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

        private void FireProjectiles(CannonInstance cannon, ShotLaunchParams parameters)
        {
            foreach (ShotPatternData patternData in cannon.Config.GetBulletPatterns())
            {
                List<ShotLaunchParams> projectiles = _patternService.ComputeSpawnPoints(patternData, parameters, cannon.ShotCount);
                
                for (int i = 0; i < patternData.burstShotCount; i++)
                {
                    _coroutine.StartCoroutine(RoutineFire(cannon, projectiles, patternData, parameters.Instigator));
                }
            }
            
            cannon.ShotCount++;
            cannon.CanShoot = false;
            cannon.IsCharged = false;
            cannon.AttackTimer = 0f;
            cannon.ChargeTimer = 0f;
        }

        private void ChargeShot(CannonInstance cannon, ShotLaunchParams parameters)
        {
            if (cannon.ChargeTimer < cannon.Config.cannonData.chargeTime)
            {
                beamPreview.UpdatePreview(parameters.SpawnPosition, GetBeamPreviewEndPoint(cannon, parameters), cannon.ChargeTimer);
                cannon.ChargeTimer += Time.deltaTime;
            }
            else
            {
                beamPreview.Despawn();
                beamPreview = null;
                cannon.IsCharged = true;
            }
        }

        private IEnumerator RoutineFire(CannonInstance cannon, List<ShotLaunchParams> projectileLaunchData, ShotPatternData patternData, GameObject instigator)
        {
            foreach (ShotLaunchParams launchData in projectileLaunchData)
            {
                cannon.CurrentFiringLogic?.Shoot(launchData.SpawnPosition, launchData.SpawnDirection, cannon.Config.cannonData, cannon.LaserCombo, instigator);
            }
            yield return new WaitForSeconds(patternData.burstShotCooldown);
        }

        public void FinishFiring(CannonInstance cannon)
        {
            if (beamPreview)
            {
                beamPreview.Despawn();
                beamPreview = null;
            }
            cannon.ChargeTimer = 0;
            cannon.CurrentFiringLogic?.FinishFiring();
        }
    }
}