using System;
using BoleteHell.Code.Gameplay.Damage.Effects;
using BoleteHell.Gameplay.Characters;
using BoleteHell.Utils.Extensions;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BoleteHell.Gameplay.StatusEffectImpl
{
    using IStatusEffectTarget = GameObject;
    [Serializable]
    public sealed class GainEnergyStatusEffectConfig : StatusEffectConfig
    {
        [MinValue(1)]
        public int EnergyEachTick = 10;
    }
    
    public sealed class GainEnergyStatusEffect : IStatusEffect<GainEnergyStatusEffectConfig>
    {
        public bool CanApply(IStatusEffectTarget target, GainEnergyStatusEffectConfig config)
        {
            return target.HasComponent<EnergyComponent>();
        }

        public void Apply(IStatusEffectTarget target, GainEnergyStatusEffectConfig config)
        {
            target.GetComponent<EnergyComponent>().GainFixedAmount(config.EnergyEachTick);
        }

        public void Unapply(IStatusEffectTarget target, GainEnergyStatusEffectConfig config)
        {
            target.GetComponent<EnergyComponent>().LoseFixedAmount(config.EnergyEachTick);
        }
    }
}