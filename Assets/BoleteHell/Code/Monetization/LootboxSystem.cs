using System;
using System.Collections.Generic;
using UnityEngine;

namespace BoleteHell.Code.Monetization
{
    /// <summary>
    /// LOOTBOX/GACHA SYSTEM (Optional but highly profitable)
    /// Randomized rewards with rarity tiers
    /// Pity system to avoid gambling regulation issues
    /// WARNING: Ethically questionable but financially proven
    /// </summary>
    public class LootboxSystem : MonoBehaviour
    {
        [SerializeField]
        private List<LootboxType> lootboxes;
        
        [SerializeField]
        private int pityCounter = 0; // Guaranteed legendary after X opens
        
        [SerializeField]
        private int pityThreshold = 50; // Industry standard ~50-100
        
        private System.Random rng = new System.Random();
        
        public LootboxRewards OpenLootbox(LootboxType lootbox)
        {
            pityCounter++;
            
            // Pity system - guarantee good reward eventually
            bool forceLegendary = pityCounter >= pityThreshold;
            
            LootboxRewards rewards = GenerateRewards(lootbox, forceLegendary);
            
            if (rewards.HighestRarity >= Rarity.Legendary)
            {
                pityCounter = 0; // Reset pity
            }
            
            // Track for analytics
            TrackLootboxOpen(lootbox.BoxId, rewards);
            
            return rewards;
        }
        
        private LootboxRewards GenerateRewards(LootboxType lootbox, bool forceLegendary)
        {
            LootboxRewards rewards = new LootboxRewards();
            
            for (int i = 0; i < lootbox.RewardCount; i++)
            {
                Rarity rarity = forceLegendary && i == 0 
                    ? Rarity.Legendary 
                    : RollRarity(lootbox.RarityWeights);
                
                var reward = GetRandomRewardOfRarity(rarity);
                rewards.Rewards.Add(reward);
            }
            
            return rewards;
        }
        
        private Rarity RollRarity(RarityWeights weights)
        {
            float roll = (float)rng.NextDouble() * 100f;
            
            // Weighted random
            if (roll < weights.MythicChance) return Rarity.Mythic;
            if (roll < weights.MythicChance + weights.LegendaryChance) return Rarity.Legendary;
            if (roll < weights.MythicChance + weights.LegendaryChance + weights.EpicChance) return Rarity.Epic;
            if (roll < 100f - weights.CommonChance) return Rarity.Rare;
            return Rarity.Common;
        }
        
        private LootboxReward GetRandomRewardOfRarity(Rarity rarity)
        {
            // Select random item from pool of given rarity
            return new LootboxReward();
        }
        
        private void TrackLootboxOpen(string boxId, LootboxRewards rewards)
        {
            // Analytics: Track what players get
            // Used to tune drop rates for maximum revenue
        }
        
        public void GrantFreeChest()
        {
            // Free chest every 4 hours - retention mechanic
            // Creates habit loop: check in -> open chest -> play game
        }
    }
    
    [Serializable]
    public class LootboxType
    {
        [field: SerializeField]
        public string BoxId { get; private set; }
        
        [field: SerializeField]
        public string DisplayName { get; private set; }
        
        [field: SerializeField]
        public CurrencyPrice Cost { get; private set; }
        
        [field: SerializeField]
        public int RewardCount { get; private set; } = 3;
        
        [field: SerializeField]
        public RarityWeights RarityWeights { get; private set; }
        
        [field: SerializeField]
        public bool CanBeFree { get; private set; } // Free daily chest
    }
    
    [Serializable]
    public struct RarityWeights
    {
        // Percentages - tuned for maximum engagement vs frustration
        [Range(0, 100)] public float CommonChance;   // 50% typical
        [Range(0, 100)] public float RareChance;     // 30%
        [Range(0, 100)] public float EpicChance;     // 15%
        [Range(0, 100)] public float LegendaryChance; // 4.5%
        [Range(0, 100)] public float MythicChance;    // 0.5% (whale bait)
    }
    
    [Serializable]
    public class LootboxRewards
    {
        public List<LootboxReward> Rewards = new List<LootboxReward>();
        public Rarity HighestRarity => GetHighestRarity();
        
        private Rarity GetHighestRarity()
        {
            Rarity highest = Rarity.Common;
            foreach (var reward in Rewards)
            {
                if (reward.ItemRarity > highest)
                    highest = reward.ItemRarity;
            }
            return highest;
        }
    }
    
    [Serializable]
    public class LootboxReward
    {
        public string ItemId;
        public Rarity ItemRarity;
        public int Quantity;
    }
}
using System;
using System.Collections.Generic;
using BoleteHell.Code.Monetization;
using BoleteHell.Code.Progression;
using UnityEngine;
using Zenject;

namespace BoleteHell.Code.Store
{
    /// <summary>
    /// IN-GAME STORE - Primary monetization interface
    /// Converts free players to paying customers
    /// Multiple psychological triggers: FOMO, scarcity, social proof
    /// </summary>
    public class GameStore : MonoBehaviour
    {
        [Inject]
        private PlayerProgressionManager _progressionManager;
        
        [SerializeField]
        private List<StoreOffer> featuredOffers;
        
        [SerializeField]
        private List<StoreOffer> dailyDeals; // Rotates daily - urgency
        
        [SerializeField]
        private List<StoreOffer> limitedTimeOffers; // FOMO driver
        
        // Analytics tracking
        public event Action<string, int> OnPurchaseCompleted;
        public event Action<string> OnPurchaseFailed;
        
        public void Initialize()
        {
            RefreshDailyDeals();
            CheckLimitedOfferExpiry();
        }
        
        public bool PurchaseOffer(StoreOffer offer)
        {
            // Validate currency
            if (!_progressionManager.Wallet.CanAfford(offer.Price))
            {
                OnPurchaseFailed?.Invoke("Insufficient currency");
                ShowCurrencyPurchasePrompt(); // Conversion funnel
                return false;
            }
            
            // Process purchase
            if (_progressionManager.Wallet.Spend(offer.Price))
            {
                GrantOfferRewards(offer);
                
                // Track for analytics
                OnPurchaseCompleted?.Invoke(offer.OfferId, offer.Price.HardCost);
                
                // Check for first purchase bonus
                if (offer.IsFirstPurchaseBonus)
                {
                    GrantFirstPurchaseExtras();
                }
                
                return true;
            }
            
            return false;
        }
        
        private void GrantOfferRewards(StoreOffer offer)
        {
            foreach (var reward in offer.Rewards)
            {
                switch (reward.Type)
                {
                    case RewardType.SoftCurrency:
                        _progressionManager.Wallet.AddSoft(reward.Amount);
                        break;
                    case RewardType.HardCurrency:
                        _progressionManager.Wallet.AddHard(reward.Amount);
                        break;
                    case RewardType.ShieldSkin:
                        UnlockSkin(reward.RewardId);
                        break;
                    // ... other reward types
                }
            }
        }
        
        private void UnlockSkin(string skinId)
        {
            // Unlock skin in player inventory
        }
        
        private void ShowCurrencyPurchasePrompt()
        {
            // "Not enough Spores! Purchase more?"
            // Direct to hard currency store
        }
        
        private void GrantFirstPurchaseExtras()
        {
            // Double rewards on first purchase - conversion boost
            _progressionManager.Wallet.AddHard(500);
        }
        
        private void RefreshDailyDeals()
        {
            // Rotate deals based on date
            // Personalized based on player behavior (if using backend)
        }
        
        private void CheckLimitedOfferExpiry()
        {
            // Remove expired offers
            // Add urgency timers
        }
    }
    
    [Serializable]
    public class StoreOffer
    {
        [field: SerializeField]
        public string OfferId { get; private set; }
        
        [field: SerializeField]
        public string DisplayName { get; private set; }
        
        [field: SerializeField]
        public string Description { get; private set; }
        
        [field: SerializeField]
        public Sprite Icon { get; private set; }
        
        [field: SerializeField]
        public CurrencyPrice Price { get; private set; }
        
        [field: SerializeField]
        public List<BattlePassReward> Rewards { get; private set; }
        
        // FOMO mechanics
        [field: SerializeField]
        public bool IsLimitedTime { get; private set; }
        
        [field: SerializeField]
        public DateTime ExpiryDate { get; private set; }
        
        [field: SerializeField]
        public int StockRemaining { get; private set; } = -1; // -1 = unlimited
        
        // Value proposition
        [field: SerializeField]
        public int PercentageDiscount { get; private set; } // "70% OFF!"
        
        [field: SerializeField]
        public bool ShowAsBestValue { get; private set; } // UI badge
        
        [field: SerializeField]
        public bool IsFirstPurchaseBonus { get; private set; }
        
        // Social proof
        [field: SerializeField]
        public int PurchaseCount { get; private set; } // "1,234 players bought this!"
        
        // Tags for filtering
        [field: SerializeField]
        public List<string> Tags { get; private set; } // "NEW", "POPULAR", "LIMITED"
    }
    
    /// <summary>
    /// Hard currency purchase options - Real money
    /// Tiered pricing strategy
    /// </summary>
    [Serializable]
    public class HardCurrencyPack
    {
        [field: SerializeField]
        public string PackId { get; private set; }
        
        [field: SerializeField]
        public int HardCurrencyAmount { get; private set; }
        
        [field: SerializeField]
        public float RealMoneyPrice { get; private set; } // USD
        
        [field: SerializeField]
        public int BonusCurrency { get; private set; } // "Buy 1000, get 300 FREE!"
        
        [field: SerializeField]
        public bool IsStarterPack { get; private set; } // One-time purchase, massive value
        
        [field: SerializeField]
        public bool ShowAsPopular { get; private set; } // "Most Popular!"
        
        // Pricing tiers standard in industry:
        // $0.99 - 100 currency (poor value - anchor)
        // $4.99 - 600 currency (starter pack, best value per dollar)
        // $9.99 - 1200 currency + 300 bonus
        // $19.99 - 2500 currency + 800 bonus (popular badge)
        // $49.99 - 7000 currency + 3000 bonus (whale tier)
        // $99.99 - 15000 currency + 8000 bonus (mega whale)
    }
}

