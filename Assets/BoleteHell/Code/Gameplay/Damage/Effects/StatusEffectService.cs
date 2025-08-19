using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace BoleteHell.Code.Gameplay.Damage.Effects
{
    public class StatusEffectService : IStatusEffectService, ITickable
    {
        [Inject]
        private List<IStatusEffect> _effects;

        private readonly SortedSet<ScheduledStatusEffect> _activeEffects = new(new ScheduledStatusEffectComparer()); 
        
        public void AddStatusEffect<T>(IDamageable target, T config) where T : StatusEffectConfig
        {
            var effect = _effects.Single(e => e.CanApply(target, config));

            // apply one shot effects
            if (config.numTicks == 1 && config.initialDelay == 0.0f)
            {
                effect.Apply(target, config);
                return;
            }
            
            // queue for application
            var instance = new ScheduledStatusEffect(effect, config, target);
            _activeEffects.Remove(instance);
            _activeEffects.Add(instance);
        }

        public void Tick()
        {
            float currentTime = Time.time;
            while (_activeEffects.Count > 0)
            {
                ScheduledStatusEffect effect = _activeEffects.Min; // earliest scheduled effect
                
                if (currentTime < effect.ScheduledTime)
                {
                    break;
                }
                
                _activeEffects.Remove(effect);
                
                if (effect.ApplyIfNeeded(currentTime))
                {
                    _activeEffects.Add(effect);// reschedule
                }
                else
                {
                    effect.UnapplyIfNeeded(); // unapply if not permanent
                }
            }
        }
    }
}
