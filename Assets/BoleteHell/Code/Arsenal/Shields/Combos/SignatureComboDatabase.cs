using System.Collections.Generic;

namespace BoleteHell.Code.Arsenal.Shields.Combos
{
    /// <summary>
    /// SIGNATURE COMBO LIBRARY - Predefined combo sequences
    /// These are the "special moves" of Bolete Hell
    /// Designed to create moments of mastery and "pro plays"
    /// Perfect for marketing, streaming, and competitive play
    /// </summary>
    public static class SignatureComboDatabase
    {
        // ============================================
        // TIER 1: BEGINNER COMBOS (Easy to execute)
        // ============================================
        
        public static ComboBonus DoubleReflect = new ComboBonus
        {
            ComboName = "Double Trouble",
            RequiredComboCount = 2,
            RequiresSpecificSequence = true,
            RequiredSequence = new List<string> { "basic_reflector", "basic_reflector" },
            BonusType = ComboBonusType.DamageMultiplier,
            BonusValue = 1.2f,
            DisplayMessage = "DOUBLE TROUBLE! +20% Damage",
            SoftCurrencyReward = 10,
            BattlePassXPReward = 5,
            AchievementToProgress = "combo_beginner"
        };
        
        public static ComboBonus RefractorChain = new ComboBonus
        {
            ComboName = "Prism Path",
            RequiredComboCount = 3,
            RequiresSpecificSequence = true,
            RequiredSequence = new List<string> { "refractor", "refractor", "refractor" },
            BonusType = ComboBonusType.DamageMultiplier,
            BonusValue = 1.3f,
            DisplayMessage = "PRISM PATH! Lasers bending reality",
            SoftCurrencyReward = 25,
            BattlePassXPReward = 10,
            AchievementToProgress = "refraction_master"
        };
        
        // ============================================
        // TIER 2: INTERMEDIATE COMBOS (Moderate skill)
        // ============================================
        
        public static ComboBonus AbsorbAndReflect = new ComboBonus
        {
            ComboName = "Energy Reversal",
            RequiredComboCount = 2,
            RequiresSpecificSequence = true,
            RequiredSequence = new List<string> { "absorber", "basic_reflector" },
            BonusType = ComboBonusType.EnergyRefund,
            BonusValue = 30f, // Refund 30 energy
            DisplayMessage = "ENERGY REVERSAL! Tank it, send it back!",
            SoftCurrencyReward = 50,
            BattlePassXPReward = 25,
            AchievementToProgress = "tactical_genius"
        };
        
        public static ComboBonus PrismChain = new ComboBonus
        {
            ComboName = "Rainbow Devastation",
            RequiredComboCount = 3,
            RequiresSpecificSequence = true,
            RequiredSequence = new List<string> { "prismatic", "prismatic", "prismatic" },
            BonusType = ComboBonusType.DamageMultiplier,
            BonusValue = 1.5f,
            DisplayMessage = "RAINBOW DEVASTATION! Screen filled with lasers!",
            SoftCurrencyReward = 100,
            BattlePassXPReward = 50,
            AchievementToProgress = "prismatic_master"
        };
        
        public static ComboBonus ReactiveBarrage = new ComboBonus
        {
            ComboName = "Auto-Revenge",
            RequiredComboCount = 4,
            RequiresSpecificSequence = true,
            RequiredSequence = new List<string> { "reactive", "reactive", "reactive", "reactive" },
            BonusType = ComboBonusType.DamageMultiplier,
            BonusValue = 1.4f,
            DisplayMessage = "AUTO-REVENGE! Automatic domination",
            SoftCurrencyReward = 75,
            BattlePassXPReward = 35,
            AchievementToProgress = "reactive_specialist"
        };
        
        // ============================================
        // TIER 3: ADVANCED COMBOS (High skill)
        // ============================================
        
        public static ComboBonus TemporalLoop = new ComboBonus
        {
            ComboName = "Time Master",
            RequiredComboCount = 3,
            RequiresSpecificSequence = true,
            RequiredSequence = new List<string> { "temporal", "quantum", "temporal" },
            BonusType = ComboBonusType.TemporaryInvulnerability,
            BonusValue = 2f, // 2 seconds invuln
            DisplayMessage = "TIME MASTER! Reality bends to your will",
            SoftCurrencyReward = 200,
            BattlePassXPReward = 100,
            AchievementToProgress = "time_lord"
        };
        
        public static ComboBonus QuantumChaos = new ComboBonus
        {
            ComboName = "Dimensional Rift",
            RequiredComboCount = 4,
            RequiresSpecificSequence = true,
            RequiredSequence = new List<string> { "quantum", "quantum", "prismatic", "quantum" },
            BonusType = ComboBonusType.DamageMultiplier,
            BonusValue = 2.0f,
            DisplayMessage = "DIMENSIONAL RIFT! Projectiles everywhere!",
            SoftCurrencyReward = 250,
            BattlePassXPReward = 150,
            AchievementToProgress = "quantum_physicist"
        };
        
        public static ComboBonus AdaptiveEvolution = new ComboBonus
        {
            ComboName = "Perfect Adaptation",
            RequiredComboCount = 5,
            RequiresSpecificSequence = true,
            RequiredSequence = new List<string> { "adaptive", "adaptive", "adaptive", "adaptive", "adaptive" },
            BonusType = ComboBonusType.TemporaryInvulnerability,
            BonusValue = 3f, // 3 seconds invuln
            DisplayMessage = "PERFECT ADAPTATION! Immune to all!",
            SoftCurrencyReward = 300,
            BattlePassXPReward = 200,
            AchievementToProgress = "evolution_master"
        };
        
        public static ComboBonus AbsorbingVoid = new ComboBonus
        {
            ComboName = "Black Hole",
            RequiredComboCount = 5,
            RequiresSpecificSequence = true,
            RequiredSequence = new List<string> { "absorber", "absorber", "absorber", "temporal", "absorber" },
            BonusType = ComboBonusType.EnergyRefund,
            BonusValue = 100f, // Massive energy refund
            DisplayMessage = "BLACK HOLE! Energy overflowing!",
            SoftCurrencyReward = 350,
            BattlePassXPReward = 250,
            AchievementToProgress = "energy_god"
        };
        
        // ============================================
        // TIER 4: MASTER COMBOS (Pro level)
        // ============================================
        
        public static ComboBonus TheArchitect = new ComboBonus
        {
            ComboName = "The Architect",
            RequiredComboCount = 7,
            RequiresSpecificSequence = true,
            RequiredSequence = new List<string> 
            { 
                "basic_reflector", 
                "refractor", 
                "absorber", 
                "prismatic", 
                "temporal", 
                "quantum", 
                "adaptive" 
            },
            BonusType = ComboBonusType.UltimateCharge,
            BonusValue = 100f, // Full ultimate charge
            DisplayMessage = "THE ARCHITECT! All 7 archetypes mastered!",
            SoftCurrencyReward = 500,
            BattlePassXPReward = 500,
            AchievementToProgress = "grand_master"
        };
        
        public static ComboBonus PerfectDefense = new ComboBonus
        {
            ComboName = "Impenetrable Fortress",
            RequiredComboCount = 10,
            RequiresSpecificSequence = false, // Any 10 shields
            BonusType = ComboBonusType.TemporaryInvulnerability,
            BonusValue = 5f, // 5 seconds invuln
            DisplayMessage = "IMPENETRABLE FORTRESS! Nothing can touch you!",
            SoftCurrencyReward = 1000,
            BattlePassXPReward = 1000,
            AchievementToProgress = "legendary_defender"
        };
        
        public static ComboBonus TimeStopMaster = new ComboBonus
        {
            ComboName = "ZA WARUDO!", // JoJo reference for culture
            RequiredComboCount = 5,
            RequiresSpecificSequence = true,
            RequiredSequence = new List<string> { "temporal", "temporal", "temporal", "temporal", "temporal" },
            BonusType = ComboBonusType.TemporaryInvulnerability,
            BonusValue = 4f,
            DisplayMessage = "ZA WARUDO! Time has stopped!",
            SoftCurrencyReward = 400,
            BattlePassXPReward = 300,
            AchievementToProgress = "time_stopper"
        };
        
        // ============================================
        // TIER 5: GODLIKE COMBOS (Whale/Esports)
        // ============================================
        
        public static ComboBonus TheImpossible = new ComboBonus
        {
            ComboName = "The Impossible",
            RequiredComboCount = 20,
            RequiresSpecificSequence = false,
            BonusType = ComboBonusType.ExtraSoftCurrency,
            BonusValue = 5000f, // Massive currency reward
            DisplayMessage = "THE IMPOSSIBLE! You are a GOD!",
            SoftCurrencyReward = 5000,
            BattlePassXPReward = 5000,
            AchievementToProgress = "impossible_achieved"
        };
        
        public static ComboBonus UltimateMastery = new ComboBonus
        {
            ComboName = "Shield God",
            RequiredComboCount = 15,
            RequiresSpecificSequence = true,
            RequiredSequence = new List<string>
            {
                "prismatic", "quantum", "temporal",
                "prismatic", "quantum", "temporal",
                "prismatic", "quantum", "temporal",
                "absorber", "adaptive", "reactive",
                "basic_reflector", "refractor", "prismatic"
            },
            BonusType = ComboBonusType.UltimateCharge,
            BonusValue = 200f,
            DisplayMessage = "SHIELD GOD! Perfect execution of all forms!",
            SoftCurrencyReward = 10000,
            BattlePassXPReward = 10000,
            AchievementToProgress = "shield_deity"
        };
        
        // ============================================
        // SPECIAL: COMMUNITY DISCOVERED COMBOS
        // (To be added post-launch based on player creativity)
        // ============================================
        
        // Leave room for community to discover "secret" combos
        // Can be added via server-side config without updates
        
        // ============================================
        // UTILITY: Get all combos for initialization
        // ============================================
        
        public static List<ComboBonus> GetAllCombos()
        {
            return new List<ComboBonus>
            {
                // Tier 1
                DoubleReflect,
                RefractorChain,
                
                // Tier 2
                AbsorbAndReflect,
                PrismChain,
                ReactiveBarrage,
                
                // Tier 3
                TemporalLoop,
                QuantumChaos,
                AdaptiveEvolution,
                AbsorbingVoid,
                
                // Tier 4
                TheArchitect,
                PerfectDefense,
                TimeStopMaster,
                
                // Tier 5
                TheImpossible,
                UltimateMastery
            };
        }
        
        public static List<ComboBonus> GetCombosByTier(int tier)
        {
            switch (tier)
            {
                case 1: return new List<ComboBonus> { DoubleReflect, RefractorChain };
                case 2: return new List<ComboBonus> { AbsorbAndReflect, PrismChain, ReactiveBarrage };
                case 3: return new List<ComboBonus> { TemporalLoop, QuantumChaos, AdaptiveEvolution, AbsorbingVoid };
                case 4: return new List<ComboBonus> { TheArchitect, PerfectDefense, TimeStopMaster };
                case 5: return new List<ComboBonus> { TheImpossible, UltimateMastery };
                default: return new List<ComboBonus>();
            }
        }
    }
}

