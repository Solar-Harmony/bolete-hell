using System;
using System.Collections.Generic;
using UnityEngine;

namespace BoleteHell.Code.Progression
{
    /// <summary>
    /// Achievement system with daily/weekly challenges
    /// Drives daily active users (DAU) and retention
    /// Creates habitual login patterns
    /// </summary>
    [Serializable]
    public class AchievementSystem
    {
        [SerializeField]
        private List<Achievement> achievements;
        
        [SerializeField]
        private List<DailyChallenge> dailyChallenges;
        
        [SerializeField]
        private List<WeeklyChallenge> weeklyChallenges;
        
        public void CheckAchievement(string achievementId, int progress)
        {
            // Implementation for achievement tracking
        }
        
        public void ResetDailyChallenges()
        {
            // Refresh daily content - keeps players coming back
        }
    }
    
    [Serializable]
    public class Achievement
    {
        [field: SerializeField]
        public string AchievementId { get; private set; }
        
        [field: SerializeField]
        public string Title { get; private set; }
        
        [field: SerializeField]
        public string Description { get; private set; }
        
        [field: SerializeField]
        public AchievementCategory Category { get; private set; }
        
        [field: SerializeField]
        public int TargetProgress { get; private set; }
        
        [field: SerializeField]
        public int CurrentProgress { get; private set; }
        
        [field: SerializeField]
        public bool IsCompleted { get; private set; }
        
        [field: SerializeField]
        public AchievementReward Reward { get; private set; }
        
        // Tier system - multiple levels of same achievement
        [field: SerializeField]
        public int Tier { get; private set; } = 1;
        
        [field: SerializeField]
        public int MaxTier { get; private set; } = 5;
    }
    
    public enum AchievementCategory
    {
        ShieldMaster,       // Reflect X projectiles
        LaserDeflector,     // Specific to shield mechanics
        PerfectDefense,     // Don't take damage for X seconds
        ComboArtist,        // Shield combos
        Survivor,           // Survive X waves
        Collector,          // Unlock X skins
        Whale               // Spend real money (yes, really)
    }
    
    [Serializable]
    public class AchievementReward
    {
        [field: SerializeField]
        public int SoftCurrency { get; private set; }
        
        [field: SerializeField]
        public int HardCurrency { get; private set; }
        
        [field: SerializeField]
        public int BattlePassXP { get; private set; }
        
        [field: SerializeField]
        public string UnlockSkinId { get; private set; }
    }
    
    [Serializable]
    public class DailyChallenge
    {
        [field: SerializeField]
        public string ChallengeId { get; private set; }
        
        [field: SerializeField]
        public string Description { get; private set; }
        
        [field: SerializeField]
        public int TargetValue { get; private set; }
        
        [field: SerializeField]
        public AchievementReward Reward { get; private set; }
        
        [field: SerializeField]
        public DateTime ExpiryDate { get; private set; }
        
        // Pressure to complete
        [field: SerializeField]
        public bool CanRerollWithCurrency { get; private set; } = true;
        
        [field: SerializeField]
        public int RerollCost { get; private set; } = 50;
    }
    
    [Serializable]
    public class WeeklyChallenge : DailyChallenge
    {
        // Harder challenges, better rewards
    }
}
using System;
using System.Collections.Generic;
using BoleteHell.Code.Monetization;
using UnityEngine;

namespace BoleteHell.Code.Progression
{
    /// <summary>
    /// Battle Pass - Proven revenue driver (Fortnite, Apex, etc.)
    /// Dual track (free + premium) creates conversion pressure
    /// Seasonal content keeps players coming back
    /// </summary>
    [CreateAssetMenu(fileName = "BattlePass", menuName = "BoleteHell/Monetization/Battle Pass")]
    public class BattlePass : ScriptableObject
    {
        [field: SerializeField]
        public int SeasonNumber { get; private set; }
        
        [field: SerializeField]
        public string SeasonName { get; private set; }
        
        [field: SerializeField]
        public DateTime SeasonStartDate { get; private set; }
        
        [field: SerializeField]
        public DateTime SeasonEndDate { get; private set; }
        
        [field: SerializeField]
        public int PremiumPassCost { get; private set; } = 950; // Industry standard pricing
        
        [field: SerializeField]
        public List<BattlePassTier> Tiers { get; private set; }
        
        [field: SerializeField]
        public int XPPerTier { get; private set; } = 1000;
        
        // Whale mechanics
        [field: SerializeField]
        public bool AllowTierBuying { get; private set; } = true;
        
        [field: SerializeField]
        public int CostPerTier { get; private set; } = 150; // Hard currency
    }
    
    [Serializable]
    public class BattlePassTier
    {
        [field: SerializeField]
        public int TierNumber { get; private set; }
        
        [field: SerializeField]
        public BattlePassReward FreeReward { get; private set; }
        
        [field: SerializeField]
        public BattlePassReward PremiumReward { get; private set; } // Incentivize purchase
    }
    
    [Serializable]
    public class BattlePassReward
    {
        [field: SerializeField]
        public RewardType Type { get; private set; }
        
        [field: SerializeField]
        public int Amount { get; private set; }
        
        [field: SerializeField]
        public string RewardId { get; private set; } // Reference to skin, shield type, etc.
        
        [field: SerializeField]
        public Sprite RewardIcon { get; private set; }
    }
    
    public enum RewardType
    {
        SoftCurrency,
        HardCurrency,
        ShieldSkin,
        ShieldType,
        XPBoost,
        EmoteOrCosmetic,
        PrestigeToken
    }
}

