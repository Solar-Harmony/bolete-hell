using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BoleteHell.Code.Gameplay.Damage.Effects
{
    [Serializable]
    public abstract class StatusEffectConfig
    {
        [BoxGroup("Basic")]
        [Tooltip("How many time should we apply the effect during its lifetime?")]
        [MinValue(1)]
        public int numTicks = 1;
        
        [BoxGroup("Basic")]
        [Tooltip("Initial delay before the first application of the effect.")]
        [Unit(Units.Second), MinValue(0.0f)]
        public float initialDelay = 0.0f;
        
        [BoxGroup("Basic")]
        [Tooltip("Time between each application of the effect.")]
        [MinValue(0.5f), Unit(Units.Second), ShowIf("@numTicks > 1")]
        public float tickInterval = 1.0f;
        
        [BoxGroup("Basic")]
        [Tooltip("Whether the effect stays (e.g. damage over time) or is reverted on expiry (e.g. speed boost).")]
        [ShowIf("@numTicks > 1")]
        public bool isPermanent = false;
        
        [BoxGroup("Basic")]
        [ShowInInspector, ReadOnly, Unit(Units.Second)]
        private float duration => initialDelay + (numTicks - 1) * tickInterval;
    }
}