using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace BoleteHell.Code.Gameplay.Damage.Effects
{
    public class StatusEffectService : IStatusEffectService
    {
        [Inject]
        private List<IStatusEffect> _effects;

        private readonly HashSet<StatusEffectInstance> _activeEffects = new(new StatusEffectInstanceComparer());
        
        public void AddStatusEffect<T>(IDamageable target, T config) where T : StatusEffectConfig
        {
            var effect = _effects.Single(e => e.CanApply(target, config));
            var instance = new StatusEffectInstance
            {
                EndTime = Time.time + config.duration,
                Effect = effect,
                Config = config,
                Target = target
            };
            _activeEffects.Remove(instance);
            _activeEffects.Add(instance);
        }

        public void Tick()
        {
            List<StatusEffectInstance> markedForRemoval = new();
            foreach (var instance in _activeEffects)
            {
                float time = Time.time;
                if (instance.IsExpired(time))
                {
                    markedForRemoval.Add(instance);
                    continue;
                }

                if (instance.ShouldApply(time))
                {
                    instance.Effect.Apply(instance.Target, instance.Config);
                    instance.UpdateTime(time);
                }
            }
            
            foreach (var instance in markedForRemoval)
            {
                _activeEffects.Remove(instance);
            }
        }
    }
}
