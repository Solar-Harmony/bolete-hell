using System;
using BoleteHell.Code.Gameplay.Characters;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BoleteHell.Code.Gameplay.Damage.Effects
{
    [Serializable]
    public class ShieldEffect
    {
        [SerializeField][Tooltip("Side(In relation to the shield creator) affected by the effect on the shield")]
        public AffectedSide affectedSide;
        
        [field:SerializeReference] [Required]
        public StatusEffectConfig statusEffectConfig { get; private set; }
    }
}

