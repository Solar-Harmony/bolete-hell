using System;
using System.Collections;
using System.Collections.Generic;
using BoleteHell.Code.Arsenal.FiringLogic;
using BoleteHell.Code.Arsenal.RayData;
using BoleteHell.Code.Arsenal.ShotPatterns;
using BoleteHell.Code.Utils;
using UnityEngine;
using Zenject;

namespace BoleteHell.Code.Arsenal.Cannons
{
    public record ShotParams(Vector2 SpawnPosition, Vector2 SpawnDirection, GameObject Instigator);

    public class CannonInstance
    {
        public readonly LaserCombo LaserCombo;
        public readonly CannonConfig Config;
        public readonly FiringLogic.FiringLogic CurrentFiringLogic;
        public readonly ShotPattern Pattern = new();
        public int ShotCount = 0;
        public float AttackTimer = 100f;
        public float ChargeTimer = 0f;
        public bool CanShoot = false;
        public bool IsCharged = false;

        public CannonInstance(CannonConfig config)
        {
            if (config.laserDatas.Count == 0)
                throw new ArgumentException("CannonConfig must have at least one LaserData");
            
            Config = config;
            LaserCombo = new LaserCombo(config.laserDatas);
            CurrentFiringLogic = config.cannonData.firingType switch
            {
                FiringTypes.Automatic => new LaserProjectileLogic(),
                FiringTypes.Charged => new LaserBeamLogic(),
                _ => throw new ArgumentOutOfRangeException()
            };
            IsCharged = !config.cannonData.WaitBeforeFiring;
        }
    }

    public class CannonService : ICannonService
    {
        [Inject]
        private IGlobalCoroutine _coroutine;

        public void Tick(CannonInstance cannon)
        {
            if (cannon.CanShoot) 
                return;
            
            if (cannon.AttackTimer < cannon.Config.cannonData.rateOfFire)
            {
                cannon.AttackTimer += Time.deltaTime;
            }
            else
            {
                cannon.CanShoot = true;
            }
        }
        
        public void TryShoot(CannonInstance cannon, ShotParams parameters)
        {
            if (!cannon.CanShoot) return;
            
            if (!cannon.IsCharged)
            {
                ChargeShot(cannon);
                return;
            }
            
            FireProjectiles(cannon, parameters);
        }

        private void FireProjectiles(CannonInstance cannon, ShotParams parameters)
        {
            foreach (ShotPatternData patternData in cannon.Config.GetBulletPatterns())
            {
                List<ProjectileLaunchData> projectiles = cannon.Pattern.Fire(patternData, parameters, cannon.ShotCount);
                
                for (int i = 0; i < patternData.burstShotCount; i++)
                {
                    _coroutine.Launch(RoutineFire(cannon, projectiles, patternData, parameters.Instigator));
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

        private IEnumerator RoutineFire(CannonInstance cannon, List<ProjectileLaunchData> projectileLaunchData, ShotPatternData patternData, GameObject instigator)
        {
            foreach (ProjectileLaunchData launchData in projectileLaunchData)
            {
                cannon.CurrentFiringLogic?.Shoot(launchData.StartPosition, launchData.Direction, cannon.Config.cannonData, cannon.LaserCombo, instigator);
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