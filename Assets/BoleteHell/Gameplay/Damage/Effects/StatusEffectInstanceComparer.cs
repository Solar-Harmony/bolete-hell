using System.Collections.Generic;

namespace BoleteHell.Gameplay.Damage.Effects
{
    public class StatusEffectInstanceComparer : IComparer<StatusEffectInstance>
    {
        public int Compare(StatusEffectInstance x, StatusEffectInstance y)
        {
            if (ReferenceEquals(x, y)) 
                return 0;
            
            if (y is null) 
                return 1;
            
            if (x is null) 
                return -1;
            
            int timeComparison = x.ScheduledTime.CompareTo(y.ScheduledTime);
            return timeComparison != 0
                ? timeComparison
                : x.GetHashCode().CompareTo(y.GetHashCode());
        }
    }
}