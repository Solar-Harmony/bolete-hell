using System;
using BoleteHell.Code.Audio;
using BoleteHell.Code.Gameplay.Characters;
using BoleteHell.Code.Graphics;
using Sirenix.OdinInspector;
using Zenject;

namespace BoleteHell.Code.Gameplay.Damage.Effects.Impl
{
    [Serializable]
    public sealed class GainEnergyStatusEffectConfig : StatusEffectConfig
    {
        [MinValue(1)]
        public int energyEachTick = 10;
    }
    
    public sealed class GainEnergyStatusEffect : IStatusEffect<GainEnergyStatusEffectConfig>
    {
        public bool CanApply(IStatusEffectTarget target, GainEnergyStatusEffectConfig config)
        {
            return target is Character;
        }

        public void Apply(IStatusEffectTarget target, GainEnergyStatusEffectConfig config)
        {
            if (target is Character character)
            {
                character.Energy.GainFixedAmount(config.energyEachTick);
            }
        }

        public void Unapply(IStatusEffectTarget target, GainEnergyStatusEffectConfig config)
        {
            if (target is Character character)
            {
                character.Energy.LoseFixedAmount(config.energyEachTick);
            }
        }
    }
}