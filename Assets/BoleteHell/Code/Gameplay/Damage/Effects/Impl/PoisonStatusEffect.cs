using System;
using Sirenix.OdinInspector;

namespace BoleteHell.Code.Gameplay.Damage.Effects.Impl
{
    [Serializable]
    public sealed class PoisonStatusEffectConfig : StatusEffectConfig
    {
        [MinValue(1)]
        public int damageEachTick = 10;
    }
    
    public sealed class PoisonStatusEffect : IStatusEffect<PoisonStatusEffectConfig>
    {
        public void Apply(IDamageable target, PoisonStatusEffectConfig config)
        {
            target.Health.TakeDamage(config.damageEachTick);
        }
    }
}