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
    
    [Serializable]
    public class Cannon
    {
        [Required]
        [SerializeField] private List<LaserData> laserDatas;
        [Required]
        [SerializeField] public RayCannonData rayCannonData;
        
        //Permet de donner des pattern vraiment basique sans avoir a créé de pattern master mais si on veut faire des pattern plus complexe
        //et réutilisable on peut faire des pattern masters pour eux
        [SerializeField] private bool useBulletPatternMaster;
        [ShowIf("@!useBulletPatternMaster")]
        [Required]
        [SerializeReference] private List<BulletPatternData> bulletPatterns;
        [ShowIf("useBulletPatternMaster")] 
        [Required]
        [SerializeField] private BulletPatternMaster bulletPatternMaster;
        
        private CombinedLaser _combinedLaser;
        private FiringLogic _currentFiringLogic;
        //Nécéssaire pour la création des Cannon a partir de l'éditeur
        public Cannon()
        {

        }
        
        //TODO: Aucune idées si la création de raycannons par code fonctionne 
        //Pourrais être utile si on vaut faire que les ennemis on des weapon semi-random
        //genre les sniper ennemis on un sniper mais qui peut avoir un laser different par ennemis
        public Cannon(List<LaserData> laserDatas,RayCannonData rayCannonData,BulletPatternMaster bulletPatternData)
        {
            useBulletPatternMaster = true;
            this.laserDatas = laserDatas;
            this.rayCannonData = rayCannonData;
            bulletPatternMaster = bulletPatternData;
        }
        
        public Cannon(List<LaserData> laserDatas,RayCannonData rayCannonData,List<BulletPatternData> bulletPatternData)
        {
            useBulletPatternMaster = false;
            this.laserDatas = laserDatas;
            this.rayCannonData = rayCannonData;
            bulletPatterns = bulletPatternData;
        }

        public void Init()
        {
            //TODO:
            //cloner les laser data
            
            //J'ai envi d'avoir des stats différent de laser on hit selon le firing type du weapon qui l'equip
            //Donc si on tire un laser qui explose,
            //de base l'explosion ferais plus de dégat et serais plus gros pour les laserBeams que les projectile
            //Peut-être avoir un Firing type enum dans le laser data qui permet de dire quel stats utiliser selon lenum
            //Scale pas pentoute mais j'ai pas vraiment l'intention d'ajouter des type de firing
            
            if (laserDatas.Count == 0)
            {
                Debug.LogWarning("No laser data provided for RayCannon!");
            }

            _combinedLaser = new CombinedLaser(laserDatas);
            
            _currentFiringLogic = rayCannonData.firingType switch
            {
                FiringTypes.Automatic => new LaserProjectileLogic(),
                FiringTypes.Charged => new LaserBeamLogic(),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public void Shoot(Vector3 startPosition, Vector3 direction, GameObject instigator = null)
        {
            _currentFiringLogic.Shoot(startPosition,direction, rayCannonData, _combinedLaser, instigator);
        }

        public void FinishFiring()
        {
            _currentFiringLogic.FinishFiring();
        }

        public List<BulletPatternData> GetBulletPatterns()
        {
            return useBulletPatternMaster ? bulletPatternMaster.patterns : bulletPatterns;
        }
    }
}