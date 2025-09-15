using System;
using BoleteHell.Code.Audio.BoleteHell.Models;
using UnityEngine;

namespace BoleteHell.Gameplay.Damage.Effects.Impl
{
    [Serializable]
    public sealed class ShrinkStatusEffectConfig : StatusEffectConfig
    {
        public float shrinkPercent = 0.5f;
    }
    
    public sealed class ShrinkStatusEffect : IStatusEffect<ShrinkStatusEffectConfig>
    {
        public bool CanApply(IStatusEffectTarget target, ShrinkStatusEffectConfig config)
        {
            return target is IMovable;
        }

        public void Apply(IStatusEffectTarget target, ShrinkStatusEffectConfig config)
        {
            var go = (MonoBehaviour)target;
            go.transform.localScale *= (1 - config.shrinkPercent);
        }

        public void Unapply(IStatusEffectTarget target, ShrinkStatusEffectConfig config)
        {
            var go = (MonoBehaviour)target;
            go.transform.localScale /= (1 - config.shrinkPercent);
        }
    }
}