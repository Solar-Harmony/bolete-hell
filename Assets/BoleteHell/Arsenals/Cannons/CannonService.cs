using System.Collections;
using System.Collections.Generic;
using BoleteHell.Arsenals.ShotPatterns;
using UnityEngine;
using Utils.BoleteHell.Utils;
using Zenject;

namespace BoleteHell.Arsenals.Cannons
{
    public record ShotLaunchParams(Vector2 SpawnPosition, Vector2 SpawnDirection, GameObject Instigator);

    public class CannonService : ICannonService
    {
        [Inject]
        private ICoroutineProvider _coroutine;
        
        [Inject]
        private IShotPatternService _patternService;

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
        
        public void TryShoot(CannonInstance cannon, ShotLaunchParams parameters)
        {
            if (!cannon.CanShoot) return;
            
            if (!cannon.IsCharged)
            {
                ChargeShot(cannon);
                return;
            }
            
            FireProjectiles(cannon, parameters);
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

        private void ChargeShot(CannonInstance cannon)
        {
            if (cannon.ChargeTimer < cannon.Config.cannonData.chargeTime)
            {
                cannon.ChargeTimer += Time.deltaTime;
            }
            else
            {
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
            cannon.ChargeTimer = 0;
            cannon.CurrentFiringLogic?.FinishFiring();
        }
    }
}