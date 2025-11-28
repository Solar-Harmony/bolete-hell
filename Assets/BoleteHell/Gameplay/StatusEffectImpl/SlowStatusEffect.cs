using System;
using BoleteHell.Code.Gameplay.Damage.Effects;
using BoleteHell.Gameplay.Characters;
using BoleteHell.Utils.Extensions;
using UnityEngine;

namespace BoleteHell.Gameplay.StatusEffectImpl
{
    [Serializable]
    public sealed class SlowStatusEffectConfig : StatusEffectConfig
    {
        public float SlowPercentage = 0.5f;
    }
    
    public sealed class SlowStatusEffect : IStatusEffect<SlowStatusEffectConfig>
    {
        public bool CanApply(GameObject target, SlowStatusEffectConfig config)
        {
            return target.HasComponent<MovementComponent>();
        }

        public void Apply(GameObject target, SlowStatusEffectConfig config)
        {
            target.GetComponent<MovementComponent>().MovementSpeed *= (1 - config.SlowPercentage);
        }

        public void Unapply(GameObject target, SlowStatusEffectConfig config)
        {
            target.GetComponent<MovementComponent>().MovementSpeed /= (1 - config.SlowPercentage);
        }
    }
}