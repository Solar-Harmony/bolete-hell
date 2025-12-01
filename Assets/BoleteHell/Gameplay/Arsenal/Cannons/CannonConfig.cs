using System;
using System.Collections.Generic;
using BoleteHell.Code.Arsenal.RayData;
using BoleteHell.Code.Arsenal.ShotPatterns;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BoleteHell.Code.Arsenal.Cannons
{
    public enum FiringTypes
    {
        Automatic, // Plein de petits lasers
        Charged,   // Charge puis tire un laser instant
    }
    
    [Serializable]
    public class Cannon
    {
        [SerializeField]
        [ValidateInput("@cannonConfigs.Count > 0", "Must have at least one cannon config.")]
        public List<CannonConfig> cannonConfigs = new();
    }
    
    [Serializable]
    public record CannonConfig
    {
        [ValidateInput("@laserDatas.Count > 0", "Must have at least one laser data.")] [SerializeField]
        public List<LaserData> laserDatas = new();
        
        [Required] [SerializeField] 
        public CannonData cannonData;
        
        [SerializeField] 
        public bool usePatternMaster;
        
        [SerializeField] [ShowIf("@!usePatternMaster")] 
        [ValidateInput(nameof(bulletPatternsValid), "At least one pattern is required.")] 
        public List<ShotPatternData> bulletPatterns = new();
        private bool bulletPatternsValid => !usePatternMaster || bulletPatterns.Count > 0;
        
        [Required] [SerializeField] [ShowIf("usePatternMaster")]
        public ShotPatternMaster shotPatternMaster;
        
        public List<ShotPatternData> GetBulletPatterns()
        {
            return usePatternMaster ? shotPatternMaster.patterns : bulletPatterns;
        }
    }
}