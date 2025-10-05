using System;
using BoleteHell.Code.Gameplay.Characters;

namespace BoleteHell.Code.Gameplay.Damage.Effects.Impl
{
    [Serializable]
    public sealed class SlowStatusEffectConfig : StatusEffectConfig
    {
        public float slowPercentage = 0.5f;
    }
    
    public sealed class SlowStatusEffect : IStatusEffect<SlowStatusEffectConfig>
    {
        public bool CanApply(IStatusEffectTarget target, SlowStatusEffectConfig config)
        {
            return target is IMovable;
        }

        public void Apply(IStatusEffectTarget target, SlowStatusEffectConfig config)
        {
            var movable = (IMovable)target;
            movable.MovementSpeed *= (1 - config.slowPercentage);
        }

        public void Unapply(IStatusEffectTarget target, SlowStatusEffectConfig config)
        {
            var movable = (IMovable)target;
            movable.MovementSpeed /= (1 - config.slowPercentage);
        }
    }
}