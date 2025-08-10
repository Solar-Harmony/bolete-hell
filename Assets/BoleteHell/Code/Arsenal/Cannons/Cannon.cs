using System;
using System.Collections.Generic;
using BoleteHell.Code.Arsenal.FiringLogic;
using BoleteHell.Code.Arsenal.RayData;
using BoleteHell.Code.Arsenal.ShotPatterns;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BoleteHell.Code.Arsenal.Cannons
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
        [ValidateInput("@laserDatas.Count > 0", "Must have at least one laser data.")] [SerializeField]
        private List<LaserData> laserDatas;
        
        [Required] [SerializeField] 
        public CannonData cannonData;
        
        //Permet de donner des pattern vraiment basique sans avoir a créé de pattern master mais si on veut faire des pattern plus complexe
        //et réutilisable on peut faire des pattern masters pour eux
        [SerializeField] 
        private bool usePatternMaster;
        
        [SerializeReference] [ShowIf("@!usePatternMaster")] [HideReferenceObjectPicker] 
        [ValidateInput("@bulletPatterns.Count > 0", "At least one pattern is required.")] 
        private List<ShotPatternData> bulletPatterns;
        
        [Required] [SerializeField] [ShowIf("usePatternMaster")]
        private ShotPatternMaster shotPatternMaster;
        
        private LaserCombo _laserCombo;
        private FiringLogic.FiringLogic _currentFiringLogic;
        
        //Nécéssaire pour la création des Cannon a partir de l'éditeur
        public Cannon()
        {
        }
        
        //TODO: Aucune idées si la création de raycannons par code fonctionne 
        //Pourrais être utile si on vaut faire que les ennemis on des weapon semi-random
        //genre les sniper ennemis on un sniper mais qui peut avoir un laser different par ennemis
        public Cannon(List<LaserData> laserDatas,CannonData cannonData,ShotPatternMaster shotPatternData)
        {
            usePatternMaster = true;
            this.laserDatas = laserDatas;
            this.cannonData = cannonData;
            shotPatternMaster = shotPatternData;
        }
        
        public Cannon(List<LaserData> laserDatas,CannonData cannonData,List<ShotPatternData> bulletPatternData)
        {
            usePatternMaster = false;
            this.laserDatas = laserDatas;
            this.cannonData = cannonData;
            bulletPatterns = bulletPatternData;
        }

        public void Init()
        {
            //TODO:
            //cloner les laser data
            
            // J'ai envie d'avoir des stats de laser on hit différentes selon le firing type du weapon qui l'équipe.
            // Donc si on tire un laser qui explose, l'explosion ferait de base plus de dégât et serais plus grosse pour les laserBeams que les projectiles.
            // Peut-être devrions-nous avoir un enum FiringType dans le laser data qui permet de dire quel stats utiliser selon l'enum
            // Ça ne scale pas pantoute mais j'ai pas vraiment l'intention d'ajouter d'autres type de firing.
            if (laserDatas.Count == 0)
                return;

            _laserCombo = new LaserCombo(laserDatas);
            
            _currentFiringLogic = cannonData.firingType switch
            {
                FiringTypes.Automatic => new LaserProjectileLogic(),
                FiringTypes.Charged => new LaserBeamLogic(),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public void Shoot(Vector3 startPosition, Vector3 direction, GameObject instigator = null)
        {
            _currentFiringLogic?.Shoot(startPosition,direction, cannonData, _laserCombo, instigator);
        }

        public void FinishFiring()
        {
            _currentFiringLogic?.FinishFiring();
        }

        public List<ShotPatternData> GetBulletPatterns()
        {
            return usePatternMaster ? shotPatternMaster.patterns : bulletPatterns;
        }
    }
}