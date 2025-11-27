using System;

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