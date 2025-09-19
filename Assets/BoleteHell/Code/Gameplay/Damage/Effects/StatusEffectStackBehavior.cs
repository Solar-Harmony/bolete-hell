using System;
using System.ComponentModel;

namespace BoleteHell.Code.Gameplay.Damage.Effects
{
    [Serializable]
    public abstract class StatusEffectStackBehavior
    {
        [Serializable]
        public class Ignore : StatusEffectStackBehavior
        {
            
        }
        
        [Serializable]
        public class Replace : StatusEffectStackBehavior
        {
            public StatusEffectStackCondition condition = StatusEffectStackCondition.Always;
        }
        
        [Serializable]
        [DisplayName("Rip bozo")]
        public class Stacking : StatusEffectStackBehavior
        {
            public int maxStacks = 3;
            public StatusEffectStackCondition condition = StatusEffectStackCondition.Always;
        }
    }

    public enum StatusEffectStackCondition
    {
        Always,
        OnlyIfBetter,
        OnlyIfWorse
    }
}