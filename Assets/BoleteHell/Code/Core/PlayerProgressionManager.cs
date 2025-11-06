using System;
using System.Collections.Generic;
using BoleteHell.Code.Monetization;
using BoleteHell.Code.Progression;
using UnityEngine;
using Zenject;

namespace BoleteHell.Code.Core
{
    /// <summary>
    /// PLAYER PROGRESSION MANAGER
    /// Tracks all player stats, currencies, unlocks
    /// Persistent across sessions - creates long-term investment
    /// </summary>
    public class PlayerProgressionManager : MonoBehaviour
    {
        [SerializeField]
        private int playerLevel = 1;
        
        [SerializeField]
        private int playerXP = 0;
        
        [SerializeField]
        private int prestigeLevel = 0; // End-game progression
        
        [field: SerializeField]
        public CurrencyWallet Wallet { get; private set; }
        
        [Inject]
        private ShieldUpgradeSystem _shieldUpgrades;
        
        [Inject]
        private BattlePass _currentBattlePass;
        
        [Inject]
        private AchievementSystem _achievements;
        
        // Stats for leaderboards and analytics
        [SerializeField]
        private PlayerStats stats;
        
        // Gacha/Lootbox state (optional monetization)
        [SerializeField]
        private DateTime lastFreeChestTime;
        
        [SerializeField]
        private int pityCounter = 0; // Guarantee legendary after X fails
        
        public event Action<int> OnLevelUp;
        public event Action<int> OnPrestige;
        public event Action<string> OnUnlockAchievement;
        
        private void Start()
        {
            LoadProgress();
        }
        
        public void AddXP(int amount)
        {
            playerXP += amount;
            CheckLevelUp();
            SaveProgress();
        }
        
        private void CheckLevelUp()
        {
            int xpRequired = CalculateXPForLevel(playerLevel + 1);
            if (playerXP >= xpRequired)
            {
                playerLevel++;
                playerXP -= xpRequired;
                OnLevelUp?.Invoke(playerLevel);
                
                // Level up rewards - retention hook
                GrantLevelUpRewards();
            }
        }
        
        private int CalculateXPForLevel(int level)
        {
            // Exponential curve - keeps players grinding longer
            return Mathf.FloorToInt(100 * Mathf.Pow(level, 1.5f));
        }
        
        private void GrantLevelUpRewards()
        {
            Wallet.AddSoft(100 * playerLevel);
            
            // Every 5 levels - give hard currency (hook for taste of premium)
            if (playerLevel % 5 == 0)
            {
                Wallet.AddHard(50);
            }
            
            // Every 10 levels - unlock something special
            if (playerLevel % 10 == 0)
            {
                // Unlock new shield type or skin
            }
        }
        
        public void Prestige()
        {
            // Reset progression but gain permanent bonuses
            // Creates infinite progression loop
            if (playerLevel < 100) return; // Must be max level
            
            prestigeLevel++;
            playerLevel = 1;
            playerXP = 0;
            
            // Keep: Skins, hard currency, prestige tokens
            // Reset: Soft currency, upgrades (but they're cheaper now)
            
            Wallet.AddPrestige(1);
            OnPrestige?.Invoke(prestigeLevel);
            
            SaveProgress();
        }
        
        // MONETIZATION ANALYTICS
        public void TrackPurchase(string itemId, int hardCurrencyCost, bool wasPremiumCurrency)
        {
            // Send to analytics
            // Track conversion funnels
            // A/B test pricing
        }
        
        private void SaveProgress()
        {
            // Save to PlayerPrefs or cloud save
            // Cloud save requires account creation (retention)
        }
        
        private void LoadProgress()
        {
            // Load from persistent storage
        }
    }
    
    [Serializable]
    public class PlayerStats
    {
        // For leaderboards and achievements
        public int TotalGamesPlayed;
        public int TotalProjectilesDeflected;
        public int TotalDamageTaken;
        public int TotalDamageDealt;
        public int HighestCombo;
        public float TotalTimePlayed;
        public int TotalDeaths;
        
        // Shield-specific stats
        public int ShieldsDrawn;
        public int PerfectDeflections;
        public int LasersSplit; // Prismatic
        public float TotalEnergyAbsorbed; // Absorber
        public int QuantumTeleports;
        
        // Monetization tracking
        public float TotalMoneySpent;
        public DateTime FirstPurchaseDate;
        public DateTime LastPurchaseDate;
        public bool IsPremiumBattlePassOwner;
    }
}

namespace BoleteHell.Code.Arsenal.Shields.Combos
{
    /// <summary>
    /// SHIELD COMBO SYSTEM - The unique selling point
    /// Rewards skilled players who chain shields strategically
    /// Creates skill expression and "pro player moments"
    /// Perfect for highlight reels and marketing
    /// </summary>
    public class ShieldComboTracker : MonoBehaviour
    {
        [SerializeField]
        private float comboWindowSeconds = 2f; // Time window to continue combo
        
        [SerializeField]
        private int currentComboCount = 0;
        
        [SerializeField]
        private float comboTimer = 0f;
        
        [SerializeField]
        private List<ComboBonus> comboBonuses;
        
        private List<string> comboSequence = new List<string>(); // Track shield types used
        
        public event Action<int, ComboBonus> OnComboAchieved;
        public event Action OnComboDropped;
        
        private void Update()
        {
            if (comboTimer > 0)
            {
                comboTimer -= Time.deltaTime;
                if (comboTimer <= 0)
                {
                    DropCombo();
                }
            }
        }
        
        public void RegisterShieldHit(string shieldType, bool successfulDeflection)
        {
            if (!successfulDeflection) return;
            
            currentComboCount++;
            comboTimer = comboWindowSeconds;
            comboSequence.Add(shieldType);
            
            CheckForComboBonuses();
        }
        
        private void CheckForComboBonuses()
        {
            foreach (var bonus in comboBonuses)
            {
                if (bonus.RequiredComboCount <= currentComboCount)
                {
                    // Check sequence requirements
                    if (bonus.RequiresSpecificSequence && !MatchesSequence(bonus.RequiredSequence))
                        continue;
                    
                    OnComboAchieved?.Invoke(currentComboCount, bonus);
                }
            }
        }
        
        private bool MatchesSequence(List<string> requiredSequence)
        {
            if (comboSequence.Count < requiredSequence.Count) return false;
            
            int startIndex = comboSequence.Count - requiredSequence.Count;
            for (int i = 0; i < requiredSequence.Count; i++)
            {
                if (comboSequence[startIndex + i] != requiredSequence[i])
                    return false;
            }
            
            return true;
        }
        
        private void DropCombo()
        {
            if (currentComboCount > 0)
            {
                OnComboDropped?.Invoke();
            }
            
            currentComboCount = 0;
            comboSequence.Clear();
        }
        
        public int GetCurrentCombo() => currentComboCount;
        public float GetComboMultiplier()
        {
            // Exponential scaling for high combos - rewards mastery
            return 1f + (currentComboCount * 0.1f);
        }
    }
    
    [Serializable]
    public class ComboBonus
    {
        [field: SerializeField]
        public string ComboName { get; private set; }
        
        [field: SerializeField]
        public int RequiredComboCount { get; private set; }
        
        [field: SerializeField]
        public bool RequiresSpecificSequence { get; private set; }
        
        [field: SerializeField]
        public List<string> RequiredSequence { get; private set; }
        
        [field: SerializeField]
        public ComboBonusType BonusType { get; private set; }
        
        [field: SerializeField]
        public float BonusValue { get; private set; }
        
        [field: SerializeField]
        public string DisplayMessage { get; private set; }
        
        [field: SerializeField]
        public int SoftCurrencyReward { get; private set; }
        
        [field: SerializeField]
        public int BattlePassXPReward { get; private set; }
        
        // Achievement integration
        [field: SerializeField]
        public string AchievementToProgress { get; private set; }
    }
    
    public enum ComboBonusType
    {
        DamageMultiplier,
        EnergyRefund,
        TemporaryInvulnerability,
        ExtraSoftCurrency,
        BattlePassXP,
        HealthRestore,
        UltimateCharge
    }
    
    /// <summary>
    /// Named combo sequences - like fighting game combos
    /// Creates mastery goals and community sharing
    /// </summary>
    public static class SignatureComboLibrary
    {
        public static readonly ComboBonus PrismChain = new ComboBonus
        {
            // Use 3 Prismatic shields in a row - massive damage
        };
        
        public static readonly ComboBonus TemporalLoop = new ComboBonus
        {
            // Temporal -> Quantum -> Temporal - time manipulation mastery
        };
        
        public static readonly ComboBonus AbsorbAndReflect = new ComboBonus
        {
            // Absorber -> Reflector - convert energy then strike back
        };
        
        public static readonly ComboBonus AdaptiveEvolution = new ComboBonus
        {
            // Use Adaptive shield 5 times in succession - full adaptation
        };
    }
}

