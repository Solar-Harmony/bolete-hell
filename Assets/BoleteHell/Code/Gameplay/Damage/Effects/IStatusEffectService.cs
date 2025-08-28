using System.Collections.Generic;

namespace BoleteHell.Code.Gameplay.Damage.Effects
{
    public interface IStatusEffectService
    {
        IReadOnlyList<IStatusEffect> GetStatusEffects();
        IReadOnlyCollection<StatusEffectInstance> GetActiveStatusEffects();
        void AddStatusEffect<T>(IStatusEffectTarget target, T config) where T : StatusEffectConfig;
    }
}
