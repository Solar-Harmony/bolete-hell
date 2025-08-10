using System;
using Sirenix.OdinInspector;

namespace BoleteHell.Code.Gameplay.Damage.Effects
{
    [Serializable]
    public abstract class StatusEffectConfig
    {
        [MinValue(0.5f)]
        public float tickInterval = 1.0f;
        
        [MinValue(0.0f)]
        public float duration = 5.0f;
    }
}