using UnityEngine;

namespace BoleteHell.Code.Gameplay.Damage.Effects
{
    public class StatusEffectInstance
    {
        // When we should next apply the effect
        public float ScheduledTime { get; private set; }
        
        private int _ticksLeft;
        private readonly IStatusEffect _effect;
        private readonly StatusEffectConfig _config;
        private readonly IDamageable _target;

        public StatusEffectInstance(IStatusEffect effect, StatusEffectConfig config, IDamageable target)
        {
            _effect = effect;
            _config = config;
            _target = target;
            _ticksLeft = config.numTicks;
            ScheduledTime = Time.time + config.initialDelay;
        }
        
        public bool ApplyIfNeeded(float currentTime)
        {
            if (IsExpired())
                return false; // Effect has expired, don't reschedule

            if (currentTime < ScheduledTime)
                return false; // Not time to apply yet
            
            _effect.Apply(_target, _config);
            ScheduledTime = currentTime + _config.tickInterval;
            _ticksLeft--;
            
            return !IsExpired(); // Return true if there are more ticks left, false if expired
        }
        
        public void UnapplyIfNeeded()
        {
            if (_config.isTransient && _effect is ITransientStatusEffect temporaryEffect)
            {
                for (int i = 0; i < _config.numTicks; i++)
                {
                    temporaryEffect.Unapply(_target, _config);
                }
            }
        }

        private bool IsExpired() => _ticksLeft <= 0;
    }
}