using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace BoleteHell.Code.Gameplay.Damage.Effects
{
    public class StatusEffectService : IStatusEffectService, ITickable, IDisposable
    {
        [Inject]
        private List<IStatusEffect> _effects;

        private readonly SortedSet<StatusEffectInstance> _activeEffects = new(new StatusEffectInstanceComparer());

        public IReadOnlyList<IStatusEffect> GetStatusEffects()
        {
            return _effects;
        }

        public IReadOnlyCollection<StatusEffectInstance> GetActiveStatusEffects()
        {
            return _activeEffects;
        }

        public void AddStatusEffect<T>(IStatusEffectTarget target, T config) where T : StatusEffectConfig
        {
            IStatusEffect effect = _effects.Single(e => e.ConfigType == config.GetType());

            if (!effect.CanApply(target, config))
            {
                return;
            }

            // apply one shot effects
            if (config.numTicks == 1 && config.initialDelay == 0.0f)
            {
                effect.Apply(target, config);
                return;
            }

            // queue for application
            var instance = new StatusEffectInstance(effect, config, target);

            var effectCopies = _activeEffects.Where(e => e.IsSameAs(instance)).ToList();
            if (effectCopies.Any())
            {
                switch (config.stackBehavior)
                {
                    case StatusEffectStackBehavior.Ignore:
                        return;
                    case StatusEffectStackBehavior.Replace replace:
                    {
                        if (MatchesCondition(config, replace.condition, effectCopies))
                        {
                            _activeEffects.RemoveWhere(e => e.IsSameAs(instance));
                            _activeEffects.Add(instance);
                        }

                        break;
                    }
                    case StatusEffectStackBehavior.Stacking stacking:
                    {
                        if (effectCopies.Count > stacking.maxStacks)
                            break;
                        
                        if (MatchesCondition(config, stacking.condition, effectCopies))
                        {
                            _activeEffects.Add(instance);
                        }

                        break;
                    }
                }
            }
            else
            {
                _activeEffects.Add(instance);
            }
        }

        public void Tick()
        {
            float currentTime = Time.time;
            while (_activeEffects.Count > 0)
            {
                StatusEffectInstance effect = _activeEffects.Min; // earliest scheduled effect

                if (currentTime < effect.ScheduledTime)
                {
                    break;
                }
                
                _activeEffects.Remove(effect);

                if (effect.ApplyIfNeeded(currentTime))
                {
                    _activeEffects.Add(effect); // reschedule
                }
                else
                {
                    effect.UnapplyIfNeeded(); // unapply if not permanent
                }
            }
        }

        private static bool MatchesCondition(StatusEffectConfig config, StatusEffectStackCondition condition, IEnumerable<StatusEffectInstance> effectStack)
        {
            return condition switch
            {
                StatusEffectStackCondition.Always => true,
                StatusEffectStackCondition.OnlyIfBetter => effectStack.All(e => e.config.IsWeakerThan(config)),
                StatusEffectStackCondition.OnlyIfWorse => effectStack.All(e => e.config.IsStrongerThan(config)),
                _ => false
            };
        }

        public void Dispose()
        {
            _activeEffects.Clear();
        }
    }
}
