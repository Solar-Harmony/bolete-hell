using System;
using System.Collections.Generic;
using BoleteHell.Arsenals.ShotPatterns;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BoleteHell.Arsenals.Cannons
{
    public enum FiringTypes
    {
        Automatic, // Plein de petits lasers
        Charged,   // Charge puis tire un laser instant
    }
    
    // TODO: See what we can merge with the CannonData scriptable object
    // It's a matter of what we want to be reusable as a template VS unique per character
    [Serializable]
    public record CannonConfig
    {
        [ValidateInput("@laserDatas.Count > 0", "Must have at least one laser data.")] [SerializeField]
        public List<LaserData.LaserData> laserDatas;
        
        [Required] [SerializeField] 
        public CannonData cannonData;
        
        [SerializeField] 
        public bool usePatternMaster;
        
        [SerializeReference] [ShowIf("@!usePatternMaster")] 
        [ValidateInput("@bulletPatterns.Count > 0", "At least one pattern is required.")] 
        public List<ShotPatternData> bulletPatterns;
        
        [Required] [SerializeField] [ShowIf("usePatternMaster")]
        public ShotPatternMaster shotPatternMaster;
        
        public List<ShotPatternData> GetBulletPatterns()
        {
            return usePatternMaster ? shotPatternMaster.patterns : bulletPatterns;
        }
    }
}