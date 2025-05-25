using System;
using System.Collections.Generic;
using Data.Cannons;
using Data.Rays;
using Lasers;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace BoleteHell.RayCannon
{
    //Automatic: Plein de petit laser
    //Charged: Charge puis tire un laser instant
    //Constant: tire un laser de manière constante
    public enum FiringTypes
    {
        Automatic,
        Charged,
    }
    
    //Un peu plus chiant a créé dans l'éditeur mais peut être facilement créé from code pour les ennemis
    [Serializable]
    public class RayCannon
    {
        [SerializeField] private List<LaserData> _laserDatas;
        [SerializeField] private RayCannonData _rayCannonData;
        private CombinedLaser _combinedLaser;
        private RayCannonFiringLogic _currentFiringLogic;

        public RayCannon()
        {
            
        }
        
        public RayCannon(List<LaserData> laserDatas,RayCannonData rayCannonData)
        {
            _laserDatas = laserDatas;
            _rayCannonData = rayCannonData;
            Init();
        }

        public void Init()
        {
            if (_laserDatas.Count == 0)
            {
                Debug.LogWarning("No laser data provided for RayCannon!");
            }

            _combinedLaser = new CombinedLaser(_laserDatas);
            
            _currentFiringLogic = _rayCannonData.firingType switch
            {
                FiringTypes.Automatic => new LaserProjectileLogic(),
                FiringTypes.Charged => new LaserBeamLogic(),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public void Shoot(Vector3 startPosition, Vector3 direction)
        {
            _currentFiringLogic.Shoot(startPosition,direction,_rayCannonData,_combinedLaser);
        }

        public void FinishFiring()
        {
            _currentFiringLogic.FinishFiring();
        }
    }
}