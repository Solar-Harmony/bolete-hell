# BOLETE HELL - TECHNICAL IMPLEMENTATION GUIDE
## Advanced Shield Combat System & Live Service Architecture

---

## 🎮 CORE GAMEPLAY ENHANCEMENTS

### Shield Type Comparison Matrix

| Shield Type | Unlock Level | Energy Cost | Skill Floor | Skill Ceiling | Monetization Tier |
|------------|--------------|-------------|-------------|---------------|-------------------|
| Reflector  | 1 (Default)  | 10/cm       | Low         | Medium        | Free              |
| Refractor  | 5            | 15/cm       | Low         | Medium        | Free              |
| Absorber   | 10           | 20/cm       | Medium      | Very High     | Premium (Epic)    |
| Reactive   | 15           | 18/cm       | Very Low    | Low           | Premium (Rare)    |
| Adaptive   | 20           | 25/cm       | High        | Very High     | Premium (Epic)    |
| Prismatic  | 30           | 30/cm       | Medium      | Very High     | Premium (Legendary)|
| Temporal   | 40           | 35/cm       | Medium      | High          | Premium (Legendary)|
| Quantum    | 50           | 40/cm       | High        | Medium        | Premium (Mythic)  |

---

## 🔧 ARCHITECTURE OVERVIEW

### New File Structure
```
Assets/BoleteHell/Code/
├── Monetization/
│   ├── Currency.cs                 ✅ Multi-currency wallet
│   ├── ShieldSkin.cs              ✅ Cosmetic system
│   ├── BattlePass.cs              ✅ Seasonal progression
│   └── LootboxSystem.cs           ✅ Optional gacha
│
├── Progression/
│   ├── ShieldUpgradeSystem.cs     ✅ Shield progression tree
│   ├── AchievementSystem.cs       ✅ Achievements & challenges
│   └── PlayerProgressionManager.cs ✅ Central progression hub
│
├── Arsenal/Shields/
│   ├── Shield.cs                   🔄 ENHANCED
│   ├── ShieldData.cs              🔄 ENHANCED
│   ├── Advanced/
│   │   └── AdvancedShieldLogic.cs ✅ New shield types
│   └── Combos/
│       └── ShieldComboSystem.cs   ✅ Combo tracking
│
├── Store/
│   └── GameStore.cs               ✅ In-game shop
│
└── Core/
    └── PlayerProgressionManager.cs ✅ Player state management
```

---

## 🎯 INTEGRATION CHECKLIST

### Phase 1: Currency & Progression Foundation

#### 1. Add to Player.cs
```csharp
[Inject]
private PlayerProgressionManager _progressionManager;

[Inject]
private ShieldComboTracker _comboTracker;

private void Start()
{
    base.Start();
    _comboTracker.OnComboAchieved += HandleComboBonus;
}

private void HandleComboBonus(int combo, ComboBonus bonus)
{
    // Grant bonus rewards
    if (bonus.BonusType == ComboBonusType.EnergyRefund)
    {
        Energy?.Replenish(bonus.BonusValue);
    }
    
    // Visual feedback
    ShowComboUI(combo, bonus);
}
```

#### 2. Zenject Installer Setup
Create `MonetizationInstaller.cs`:
```csharp
public class MonetizationInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<PlayerProgressionManager>()
            .FromComponentInHierarchy()
            .AsSingle();
            
        Container.Bind<ShieldComboTracker>()
            .FromComponentInHierarchy()
            .AsSingle();
            
        Container.Bind<GameStore>()
            .FromComponentInHierarchy()
            .AsSingle();
    }
}
```

#### 3. Persistence System
```csharp
public class SaveSystem
{
    public void SavePlayerProgress(PlayerProgressionManager manager)
    {
        PlayerSaveData data = new PlayerSaveData
        {
            playerLevel = manager.playerLevel,
            softCurrency = manager.Wallet.SoftCurrency,
            hardCurrency = manager.Wallet.HardCurrency,
            // ... all progression data
        };
        
        string json = JsonUtility.ToJson(data);
        
        // Option 1: Local (PlayerPrefs)
        PlayerPrefs.SetString("PlayerData", json);
        
        // Option 2: Cloud save (Unity Gaming Services)
        // CloudSaveService.Instance.SaveData(json);
    }
}
```

---

## 🎨 UI IMPLEMENTATION NEEDS

### Required UI Screens

#### 1. Main Menu Additions
```
[Main Menu]
├── [PLAY] (existing)
├── [SHIELDS] ← NEW
│   ├── View all shield types
│   ├── Upgrade shields
│   └── Equip skins
├── [BATTLE PASS] ← NEW
├── [STORE] ← NEW
├── [PROFILE] ← NEW
│   ├── Stats
│   ├── Achievements
│   └── Leaderboards
└── [Settings] (existing)
```

#### 2. HUD Enhancements
```
[In-Game HUD]
├── Health bar (existing)
├── Energy bar (existing)
├── COMBO COUNTER ← NEW (top center)
│   └── Shows current combo + multiplier
├── SHIELD COOLDOWNS ← NEW (bottom)
│   └── Shows available shields + cooldown timers
└── CURRENCY DISPLAY ← NEW (top right)
```

#### 3. Battle Pass Screen
- Track display (100 tiers)
- Free vs Premium comparison
- Purchase button
- Tier skip option
- Current XP progress

#### 4. Store Screen
- Featured offers carousel
- Daily deals (rotating)
- Shield skins catalog (filterable by rarity)
- Hard currency purchase

---

## 📊 ANALYTICS INTEGRATION

### Key Events to Track

```csharp
public class AnalyticsEvents
{
    // Monetization
    public static void TrackPurchase(string itemId, float price, string currency)
    {
        // Unity Analytics / Firebase / Custom backend
        Analytics.CustomEvent("iap_purchase", new Dictionary<string, object>
        {
            { "item_id", itemId },
            { "price", price },
            { "currency", currency }
        });
    }
    
    // Engagement
    public static void TrackShieldUsage(string shieldType, int comboCount)
    {
        Analytics.CustomEvent("shield_used", new Dictionary<string, object>
        {
            { "shield_type", shieldType },
            { "combo_count", comboCount }
        });
    }
    
    // Progression
    public static void TrackLevelUp(int newLevel)
    {
        Analytics.CustomEvent("level_up", new Dictionary<string, object>
        {
            { "level", newLevel }
        });
    }
    
    // Retention
    public static void TrackDailyLogin(int consecutiveDays)
    {
        Analytics.CustomEvent("daily_login", new Dictionary<string, object>
        {
            { "consecutive_days", consecutiveDays }
        });
    }
}
```

---

## 🎮 GAMEPLAY BALANCING GUIDELINES

### Energy Economy
**Current**: 
- Max Energy: 150
- Regen Rate: 20/s
- Basic Shield Cost: 10/cm

**Proposed Adjustments**:
- Max Energy: 200 (allows more shield usage)
- Regen Rate: 25/s (faster recovery)
- Shield Costs: Tiered by power
  - Reflector: 8/cm
  - Absorber: 15/cm (but refunds energy)
  - Prismatic: 25/cm (high cost, high reward)
  - Quantum: 30/cm (most expensive)

### Combo Balance
**Combo Multipliers**:
- 3-hit combo: 1.1x damage
- 5-hit combo: 1.25x damage
- 10-hit combo: 1.5x damage
- 20-hit combo: 2.0x damage (cap)

**Combo Window**: 2 seconds (forgiving but requires skill)

---

## 🔐 ANTI-CHEAT CONSIDERATIONS

### Server-Side Validation (Recommended)
```csharp
public class SecureTransactionService
{
    // All currency changes must go through server
    public async Task<bool> ValidatePurchase(string itemId, int price)
    {
        // Server validates:
        // 1. Player has enough currency
        // 2. Item exists and price matches
        // 3. Anti-fraud checks
        
        var response = await ApiClient.Post("/validate_purchase", new {
            playerId = PlayerId,
            itemId = itemId,
            price = price
        });
        
        return response.IsValid;
    }
}
```

### Local Obfuscation (Minimum)
```csharp
// Don't store currency as plain int
public class ObfuscatedInt
{
    private int offset = UnityEngine.Random.Range(1000, 9999);
    private int value;
    
    public int Value
    {
        get => value - offset;
        set => this.value = value + offset;
    }
}
```

---

## 🎨 SHADER RECOMMENDATIONS

### Shield Visual Effects

#### 1. Prismatic Shield Shader
```glsl
// Rainbow refraction effect
// Use fresnel + chromatic aberration
// Separate R, G, B channels with slight offsets
```

#### 2. Temporal Shield Shader
```glsl
// Time distortion effect
// Ripple/wave distortion on screen space
// Desaturate area around shield
```

#### 3. Quantum Shield Shader
```glsl
// Quantum uncertainty visualization
// Particle noise + displacement
// Glitch effect on teleport
```

---

## 🎵 AUDIO DESIGN

### New Sound Requirements

**Shield Sounds**:
- Draw/create sound (per shield type)
- Hit/impact sound (per shield type)
- Break/shatter sound
- Combo milestone sounds (3, 5, 10, 20 combo)

**UI Sounds**:
- Currency spend
- Level up fanfare
- Achievement unlock
- Lootbox open sequence
- Rarity reveal stingers (Common → Mythic)

**Music**:
- Shop/menu music (calm, browsing atmosphere)
- Intense combat music with combo crescendos

---

## 📱 MOBILE CONSIDERATIONS (Future)

### Control Adaptations
- Touch and drag for shield drawing (already implemented?)
- Virtual joystick for movement
- Auto-fire option for accessibility

### Performance Targets
- 60 FPS on mid-range devices (2-3 years old)
- Reduced particle effects on low-end
- Asset bundles for download size management

### Monetization Adjustments
- Lower price points for mobile ($0.99-$9.99 range)
- Rewarded video ads as option (30% revenue boost)
- Energy refill offers (common mobile mechanic)

---

## 🧪 A/B TESTING FRAMEWORK

### What to Test

1. **Pricing**
   - Battle Pass: $9.99 vs $7.99 vs $11.99
   - Starter Pack: $4.99 vs $5.99

2. **UI Flow**
   - Shop button placement
   - Battle Pass prominence
   - Notification timing

3. **Rewards**
   - Daily challenge reward amounts
   - Free vs Premium track balance

4. **Difficulty**
   - Energy costs per shield type
   - Combo window duration

### Implementation
```csharp
public class ABTestingService
{
    public enum TestVariant { A, B, Control }
    
    public TestVariant GetVariant(string testName)
    {
        // Consistent assignment based on player ID
        int hash = PlayerId.GetHashCode() + testName.GetHashCode();
        return (TestVariant)(Mathf.Abs(hash) % 3);
    }
    
    public void TrackConversion(string testName, TestVariant variant)
    {
        Analytics.CustomEvent("ab_test_conversion", new {
            test = testName,
            variant = variant.ToString()
        });
    }
}
```

---

## 🚀 LAUNCH CHECKLIST

### Pre-Launch (Technical)
- [ ] Payment integration tested (sandbox → production)
- [ ] Save system working (local + cloud)
- [ ] Analytics events firing correctly
- [ ] Performance profiling complete
- [ ] Memory leaks resolved
- [ ] All shield types balanced
- [ ] Tutorial functional for new players
- [ ] Progression curve validated (playtest)

### Pre-Launch (Content)
- [ ] Season 1 Battle Pass complete (100 tiers)
- [ ] Minimum 20 shield skins created
- [ ] Store offers configured
- [ ] Daily/weekly challenges written
- [ ] Achievement list finalized
- [ ] Localization complete (minimum: EN, ES, FR, DE, JP, CN)

### Pre-Launch (Legal/Compliance)
- [ ] Privacy policy
- [ ] Terms of service
- [ ] COPPA compliance (if targeting <13)
- [ ] GDPR compliance (EU users)
- [ ] Lootbox disclosure (odds display)
- [ ] Age gates for purchases

### Post-Launch (Week 1)
- [ ] Monitor crash reports
- [ ] Track conversion funnel
- [ ] Community management (Discord, social)
- [ ] Hotfix pipeline ready
- [ ] First balance patch prepared

---

## 📈 LIVE OPS CALENDAR (Template)

### Weekly Cadence
**Monday**: New daily challenges refresh
**Wednesday**: Mid-week boost event (2x XP, 2 hours)
**Friday**: Weekend event starts
**Sunday**: Leaderboard results, rewards distributed

### Monthly Cadence
**Week 1**: New season begins, Battle Pass refresh
**Week 2**: Limited-time shop offer
**Week 3**: Community challenge (global goal)
**Week 4**: Season end push (FOMO messaging)

### Seasonal Events (Annual)
- Halloween: Spooky shield skins, themed challenges
- Winter: Holiday cosmetics, gift events
- Spring: Easter egg hunt event
- Summer: Beach/tropical themed content
- Anniversary: Celebration event, free rewards

---

## 🎯 KEY METRICS DASHBOARD

### Daily Monitoring
```
DAU (Daily Active Users)
MAU (Monthly Active Users)
ARPU (Average Revenue Per User)
ARPPU (Average Revenue Per Paying User)
Conversion Rate (Free → Paying)
Retention (D1, D7, D30)
Session Length
Sessions per User
```

### Monetization Funnel
```
Store Visits → 
  Item Views → 
    Add to Cart → 
      Purchase Attempt → 
        Successful Purchase
```

Track dropoff at each stage, optimize worst performers.

---

## 🔧 DEBUGGING TOOLS

### In-Game Debug Menu (Development Only)
```csharp
#if UNITY_EDITOR || DEVELOPMENT_BUILD
public class DebugMenu : MonoBehaviour
{
    private bool showMenu = false;
    
    void OnGUI()
    {
        if (GUILayout.Button("Toggle Debug Menu"))
            showMenu = !showMenu;
            
        if (showMenu)
        {
            GUILayout.Label("=== DEBUG MENU ===");
            
            if (GUILayout.Button("Add 1000 Soft Currency"))
                ProgressionManager.Wallet.AddSoft(1000);
                
            if (GUILayout.Button("Add 500 Hard Currency"))
                ProgressionManager.Wallet.AddHard(500);
                
            if (GUILayout.Button("Unlock All Shields"))
                UnlockAllShields();
                
            if (GUILayout.Button("Complete Battle Pass"))
                CompleteBattlePass();
                
            if (GUILayout.Button("Reset Progression"))
                ResetAll();
        }
    }
}
#endif
```

---

## 📚 DOCUMENTATION REQUIREMENTS

### For Development Team
- Code commenting standards
- Architecture diagrams
- API documentation (if using backend)
- Build/deploy procedures

### For Content Team
- Shield balancing spreadsheet
- Battle Pass content pipeline
- Event creation guide
- Localization workflow

### For QA Team
- Test cases for all monetization flows
- Regression test suite
- Performance benchmarks
- Edge case scenarios

---

## ✅ DONE - FILES CREATED

All core systems implemented:
- ✅ Multi-currency economy
- ✅ Battle Pass system
- ✅ Shield progression & upgrades
- ✅ Achievement & challenge framework
- ✅ Combo system
- ✅ Advanced shield types (8 archetypes)
- ✅ Cosmetic skin system
- ✅ In-game store
- ✅ Optional lootbox system
- ✅ Player progression manager

**Next Steps**:
1. Integrate with existing codebase (Zenject bindings)
2. Create UI screens
3. Balance testing & tuning
4. Content creation (skins, effects)
5. Payment integration
6. Soft launch testing

---

**Prepared by**: AI Senior Systems Designer
**Date**: November 2025
**Status**: Ready for engineering implementation

