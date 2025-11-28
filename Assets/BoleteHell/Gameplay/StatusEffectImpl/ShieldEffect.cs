using System;
using BoleteHell.Code.Gameplay.Damage.Effects;
using BoleteHell.Gameplay.Characters;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BoleteHell.Gameplay.StatusEffectImpl
{
    [Serializable]
    public class ShieldEffect
    {
        [SerializeField] [Tooltip("Side(In relation to the shield creator) affected by the effect on the shield")]
        public AffectedSide AffectedSide;
        
        [field:SerializeReference] [Required]
        public StatusEffectConfig statusEffectConfig { get; private set; }
    }
}

