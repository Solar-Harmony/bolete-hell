using System.Collections.Generic;
using UnityEngine;

namespace BoleteHell.Code.Gameplay.Damage.Effects
{
    public interface IStatusEffectService
    {
        IReadOnlyList<IStatusEffect> GetStatusEffects();
        IReadOnlyCollection<StatusEffectInstance> GetActiveStatusEffects();
        void AddStatusEffect<T>(GameObject target, T config) where T : StatusEffectConfig;
        void ClearStatusEffects(GameObject target);
    }
}
