using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BoleteHell.Code.Gameplay.Damage.Effects
{
    [Serializable]
    public abstract class StatusEffectConfig
    {
        private const string GroupName = "Status Effect";
        
        [BoxGroup(GroupName)]
        [Tooltip("Whether this effect is permanent or can be unapplied. Does nothing if unsupported.")]
        public bool isTransient = false;
        
        [BoxGroup(GroupName)]
        [Tooltip("How many time should we apply the effect during its lifetime?")]
        [MinValue(1)]
        public int numTicks = 1;
        
        [BoxGroup(GroupName)]
        [Tooltip("Initial delay before the first application of the effect.")]
        [Unit(Units.Second), MinValue(0.0f)]
        public float initialDelay = 0.0f;
        
        [BoxGroup(GroupName)]
        [Tooltip("Time between each application of the effect.")]
        [MinValue(0.5f), Unit(Units.Second), ShowIf("@numTicks > 1")]
        public float tickInterval = 1.0f;
        
        [PropertySpace(8)]
        [BoxGroup(GroupName)]
        [ShowInInspector, HideLabel, ReadOnly]
        private string info => $"This status effect will last {initialDelay + (numTicks - 1) * tickInterval} s";
    }
}