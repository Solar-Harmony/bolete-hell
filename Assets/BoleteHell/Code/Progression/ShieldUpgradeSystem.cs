using System;
using System.Collections.Generic;
using BoleteHell.Code.Monetization;
using UnityEngine;

namespace BoleteHell.Code.Progression
{
    /// <summary>
    /// Deep progression system for shields - creates long-term engagement
    /// Players unlock and upgrade different shield types with unique mechanics
    /// </summary>
    [CreateAssetMenu(fileName = "ShieldUpgradeSystem", menuName = "BoleteHell/Progression/Shield Upgrade System")]
    public class ShieldUpgradeSystem : ScriptableObject
    {
        [SerializeField]
        private List<ShieldType> availableShieldTypes;
    }
    
    [Serializable]
    public class ShieldType
    {
        [field: SerializeField]
        public string TypeName { get; private set; }
        
        [field: SerializeField]
        public string Description { get; private set; }
        
        [field: SerializeField]
        public ShieldArchetype Archetype { get; private set; }
        
        [field: SerializeField]
        public int CurrentLevel { get; private set; }
        
        [field: SerializeField]
        public int MaxLevel { get; private set; } = 50; // Long grind = retention
        
        [field: SerializeField]
        public bool IsUnlocked { get; private set; }
        
        [field: SerializeField]
        public List<ShieldUpgrade> Upgrades { get; private set; }
        
        // Unlock requirements - progression gates
        [field: SerializeField]
        public int UnlockPlayerLevel { get; private set; }
        
        [field: SerializeField]
        public CurrencyPrice UnlockCost { get; private set; }
        
        // Monetization shortcut
        [field: SerializeField]
        public bool CanBuyWithHardCurrency { get; private set; } = true;
        
        [field: SerializeField]
        public int HardCurrencyUnlockPrice { get; private set; } = 500;
    }
    
    public enum ShieldArchetype
    {
        Reflector,      // Mirror - reflects projectiles
        Refractor,      // Prism - bends lasers
        Absorber,       // Converts damage to energy - high skill ceiling
        Prismatic,      // Splits lasers into multiple beams (PREMIUM)
        Temporal,       // Slows time in radius (LEGENDARY)
        Quantum,        // Teleports projectiles (MYTHIC - whale bait)
        Adaptive,       // Changes properties based on incoming damage
        Reactive        // Counter-attacks automatically
    }
    
    [Serializable]
    public class ShieldUpgrade
    {
        [field: SerializeField]
        public int Level { get; private set; }
        
        [field: SerializeField]
        public string UpgradeName { get; private set; }
        
        [field: SerializeField]
        public CurrencyPrice Cost { get; private set; }
        
        [field: SerializeField]
        public ShieldStatModifiers StatModifiers { get; private set; }
        
        // Time gates - retention mechanic
        [field: SerializeField]
        public float UpgradeTimeHours { get; private set; } = 0; // Can skip with premium currency
        
        [field: SerializeField]
        public int SkipCostHardCurrency { get; private set; }
    }
    
    [Serializable]
    public struct ShieldStatModifiers
    {
        public float DurationMultiplier;
        public float EnergyCostMultiplier;
        public float ReflectionDamageBonus;
        public float SizeBonus;
        public int MaxSimultaneousShields; // Skill expression
        public float CooldownReduction;
        public bool UnlocksNewAbility;
        public string AbilityDescription;
    }
}
using System;
using UnityEngine;

namespace BoleteHell.Code.Monetization
{
    /// <summary>
    /// Premium shield cosmetics - key monetization driver
    /// Different visual effects, trails, and particles
    /// </summary>
    [CreateAssetMenu(fileName = "ShieldSkin", menuName = "BoleteHell/Monetization/Shield Skin")]
    public class ShieldSkin : ScriptableObject
    {
        [field: SerializeField]
        public string SkinName { get; private set; }
        
        [field: SerializeField]
        public string SkinId { get; private set; }
        
        [field: SerializeField]
        public Rarity Rarity { get; private set; }
        
        [field: SerializeField]
        public Material ShieldMaterial { get; private set; }
        
        [field: SerializeField]
        public ParticleSystem TrailEffect { get; private set; }
        
        [field: SerializeField]
        public AudioClip DrawSound { get; private set; }
        
        [field: SerializeField]
        public AudioClip HitSound { get; private set; }
        
        // Monetization flags
        [field: SerializeField]
        public bool IsPremiumOnly { get; private set; }
        
        [field: SerializeField]
        public int HardCurrencyCost { get; private set; }
        
        [field: SerializeField]
        public int BattlePassTier { get; private set; } = -1; // -1 = not battle pass exclusive
        
        [field: SerializeField]
        public bool IsSeasonalExclusive { get; private set; }
        
        [field: SerializeField]
        public string SeasonId { get; private set; }
    }
    
    public enum Rarity
    {
        Common,
        Rare,
        Epic,
        Legendary,
        Mythic,
        Limited // FOMO driver
    }
}

