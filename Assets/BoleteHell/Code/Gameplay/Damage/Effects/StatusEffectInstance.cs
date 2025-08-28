using UnityEngine;

namespace BoleteHell.Code.Gameplay.Damage.Effects
{
    public class StatusEffectInstance
    {
        // When we should next apply the effect
        public float ScheduledTime { get; private set; }
        
        private int _ticksLeft;
        private readonly IStatusEffect _effect;
        public readonly StatusEffectConfig config;
        private readonly IStatusEffectTarget _target;

        // Public properties for debugger access
        public IStatusEffect Effect => _effect;
        public IStatusEffectTarget Target => _target;
        public int TicksLeft => _ticksLeft;
        public string EffectName => _effect.GetType().Name;
        public string TargetName => _target?.ToString() ?? "Unknown";
        public float TimeRemaining => Mathf.Max(0f, ScheduledTime - Time.time);
        public bool IsExpired => _ticksLeft <= 0;

        public StatusEffectInstance(IStatusEffect effect, StatusEffectConfig config, IStatusEffectTarget target)
        {
            _effect = effect;
            this.config = config;
            _target = target;
            _ticksLeft = config.numTicks;
            ScheduledTime = Time.time + config.initialDelay;
        }
        
        public bool ApplyIfNeeded(float currentTime)
        {
            if (IsExpired)
                return false; // Effect has expired, don't reschedule

            if (currentTime < ScheduledTime)
                return false; // Not time to apply yet
            
            _effect.Apply(_target, config);
            ScheduledTime = currentTime + config.tickInterval;
            _ticksLeft--;
            
            return !IsExpired; // Return true if there are more ticks left, false if expired
        }
        
        public void UnapplyIfNeeded()
        {
            if (config.isTransient)
            {
                for (int i = 0; i < config.numTicks; i++)
                {
                    _effect.Unapply(_target, config);
                }
            }
        }

        public bool IsSameAs(StatusEffectInstance other)
        {
            return _effect == other._effect && 
                   _target == other._target && 
                   config == other.config;
        }
    }
}