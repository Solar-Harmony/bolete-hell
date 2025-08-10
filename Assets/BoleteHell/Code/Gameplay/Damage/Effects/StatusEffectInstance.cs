using System;
using System.Collections.Generic;

namespace BoleteHell.Code.Gameplay.Damage.Effects
{
    public class StatusEffectInstance
    {
        public float StartTime { get; set; }
        public float EndTime { get; init; }
        public IStatusEffect Effect { get; init; }
        public StatusEffectConfig Config { get; init; }
        public IDamageable Target { get; init; }
        
        public bool ShouldApply(float time)
        {
            return time > StartTime + Config.tickInterval;
        }
        
        public bool IsExpired(float time)
        {
            return time > EndTime;
        }
        
        public void UpdateTime(float time)
        {
            StartTime = time;
        }
    }

    public class StatusEffectInstanceComparer : IEqualityComparer<StatusEffectInstance>
    {
        public bool Equals(StatusEffectInstance x, StatusEffectInstance y)
        {
            if (ReferenceEquals(x, y)) 
                return true;
            
            if (x is null || y is null) 
                return false;
            
            return Equals(x.Effect, y.Effect) && Equals(x.Target, y.Target);
        }

        public int GetHashCode(StatusEffectInstance obj)
        {
            return HashCode.Combine(obj.Effect, obj.Target);
        }
    }
}