using System;
using System.Collections.Generic;
using BoleteHell.Code.Arsenal.FiringLogic;
using BoleteHell.Code.Arsenal.RayData;
using UnityEngine;

namespace BoleteHell.Code.Arsenal.Cannons
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
        //Mis ici car j'ai besoin d'une liste par instance de cannon
        [HideInInspector]
        public List<LaserPreviewRenderer> reservedPreviewRenderers = new ();

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