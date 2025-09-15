using System;
using BoleteHell.Arsenals.FiringLogic;
using BoleteHell.Arsenals.LaserData;

namespace BoleteHell.Arsenals.Cannons
{
    public class CannonInstance
    {
        public readonly LaserCombo LaserCombo;
        public readonly CannonConfig Config;
        public readonly FiringLogic.FiringLogic CurrentFiringLogic;
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
}