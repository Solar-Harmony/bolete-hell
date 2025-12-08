using System;
using BoleteHell.Code.Arsenal;
using BoleteHell.Code.Arsenal.Cannons;
using BoleteHell.Code.Gameplay.Damage.Effects;
using BoleteHell.Gameplay.Characters;
using BoleteHell.Utils.Extensions;
using UnityEngine;

namespace BoleteHell.Gameplay.StatusEffectImpl
{
    [Serializable]
    public sealed class SlowFireRateEffectConfig : StatusEffectConfig
    {
        public float SlowPercentage = 0.5f;
    }
    
    public class SlowFireRateEffect :IStatusEffect<SlowFireRateEffectConfig>
    {
        public bool CanApply(GameObject target, SlowFireRateEffectConfig config)
        {
            return target.HasComponent<Arsenal>();
        }

        public void Apply(GameObject target, SlowFireRateEffectConfig config)
        {
            target.GetComponent<Arsenal>().UpdateCooldownModifier(config.SlowPercentage);
        }

        //TODO: Il faudrait une manière de s'assurer que on unnaply sur les cannon qui avait été apply
        //Présentement ça va unapply sur l'arme équiper ce qui pourrait avaoir changer entre temps
        public void Unapply(GameObject target, SlowFireRateEffectConfig config)
        {
            target.GetComponent<Arsenal>().UpdateCooldownModifier(-config.SlowPercentage);
        }
    }
}
