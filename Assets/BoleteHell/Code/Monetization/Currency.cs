using System;
using UnityEngine;

namespace BoleteHell.Code.Monetization
{
    /// <summary>
    /// Multi-currency economy system - Industry standard for maximizing revenue
    /// Soft currency (earned through play) + Hard currency (premium)
    /// Creates friction and conversion opportunities
    /// </summary>
    [Serializable]
    public class CurrencyWallet
    {
        [SerializeField] 
        private int softCurrency; // "Boletes" - earned through gameplay
        
        [SerializeField] 
        private int hardCurrency; // "Spores" - premium currency (real money)
        
        [SerializeField]
        private int prestigeTokens; // End-game currency for whales
        
        [SerializeField]
        private int battlePassXP;
        
        [SerializeField]
        private int seasonalCoins; // FOMO currency that expires
        
        public int SoftCurrency => softCurrency;
        public int HardCurrency => hardCurrency;
        public int PrestigeTokens => prestigeTokens;
        public int BattlePassXP => battlePassXP;
        public int SeasonalCoins => seasonalCoins;
        
        public bool CanAfford(CurrencyPrice price)
        {
            return softCurrency >= price.SoftCost && 
                   hardCurrency >= price.HardCost &&
                   prestigeTokens >= price.PrestigeCost;
        }
        
        public bool Spend(CurrencyPrice price)
        {
            if (!CanAfford(price)) return false;
            
            softCurrency -= price.SoftCost;
            hardCurrency -= price.HardCost;
            prestigeTokens -= price.PrestigeCost;
            
            return true;
        }
        
        public void AddSoft(int amount) => softCurrency += amount;
        public void AddHard(int amount) => hardCurrency += amount;
        public void AddPrestige(int amount) => prestigeTokens += amount;
        public void AddBattlePassXP(int amount) => battlePassXP += amount;
        public void AddSeasonalCoins(int amount) => seasonalCoins += amount;
        
        // Seasonal reset - creates urgency to spend
        public void ResetSeasonalCurrency()
        {
            seasonalCoins = 0;
        }
    }
    
    [Serializable]
    public struct CurrencyPrice
    {
        public int SoftCost;
        public int HardCost;
        public int PrestigeCost;
        
        public CurrencyPrice(int soft = 0, int hard = 0, int prestige = 0)
        {
            SoftCost = soft;
            HardCost = hard;
            PrestigeCost = prestige;
        }
    }
}

