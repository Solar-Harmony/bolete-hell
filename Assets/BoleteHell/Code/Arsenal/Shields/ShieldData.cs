using System.Collections.Generic;
using BoleteHell.Code.Arsenal.Shields.ShieldsLogic;
using BoleteHell.Code.Gameplay.Damage.Effects;
using BoleteHell.Code.Monetization;
using BoleteHell.Code.Progression;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BoleteHell.Code.Arsenal.Shields
{

   

    /// <summary>
    ///     Classe qui permet de déterminer les informations spécifique a un shield
    ///     ENHANCED: Now supports progression, monetization, and advanced mechanics
    /// </summary>
    [CreateAssetMenu(fileName = "ShieldData", menuName = "BoleteHell/Arsenal/Shield Data")]
    public class ShieldData : ScriptableObject
    {
        [field: SerializeField] 
        public Color color { get; set; }

        [Required] [field:SerializeReference] 
        public IShieldHitLogic onHitLogic { get; private set; }
        
        [SerializeField]
        public float despawnTime = 3f;
        
        [field:SerializeReference] [Required]
        public List<ShieldEffect> shieldEffect { get; private set; }
        
        [field: SerializeField]
        [MinValue(0f)]
        public float EnergyCostPerCm { get; set; } = 10f;
        
        // NEW: Progression integration
        [field: SerializeField]
        [BoxGroup("Progression")]
        public ShieldArchetype Archetype { get; private set; } = ShieldArchetype.Reflector;
        
        [field: SerializeField]
        [BoxGroup("Progression")]
        public int RequiredPlayerLevel { get; private set; } = 1;
        
        [field: SerializeField]
        [BoxGroup("Progression")]
        public bool IsUnlockedByDefault { get; private set; } = true;
        
        // NEW: Monetization
        [field: SerializeField]
        [BoxGroup("Monetization")]
        public CurrencyPrice UnlockCost { get; private set; }
        
        [field: SerializeField]
        [BoxGroup("Monetization")]
        public Rarity Rarity { get; private set; } = Rarity.Common;
        
        [field: SerializeField]
        [BoxGroup("Monetization")]
        public bool IsPremiumExclusive { get; private set; } = false;
        
        // NEW: Combo system
        [field: SerializeField]
        [BoxGroup("Combo System")]
        public string ShieldTypeId { get; private set; } = "basic_reflector";
        
        [field: SerializeField]
        [BoxGroup("Combo System")]
        public int ComboScore { get; private set; } = 1; // How much it contributes to combo
        
        // NEW: Visual upgrades
        [field: SerializeField]
        [BoxGroup("Cosmetics")]
        public List<ShieldSkin> AvailableSkins { get; private set; }
        
        [field: SerializeField]
        [BoxGroup("Cosmetics")]
        public ShieldSkin DefaultSkin { get; private set; }
        
        // NEW: Advanced stats
        [field: SerializeField]
        [BoxGroup("Advanced Stats")]
        public float BaseHealth { get; private set; } = 100f; // Shield can break after absorbing damage
        
        [field: SerializeField]
        [BoxGroup("Advanced Stats")]
        public float Cooldown { get; private set; } = 0f; // Seconds before same type can be used again
        
        [field: SerializeField]
        [BoxGroup("Advanced Stats")]
        public int MaxSimultaneousInstances { get; private set; } = 1; // Can place multiple shields
    }
}