using Zenject;

namespace BoleteHell.Code.Gameplay.Damage.Effects
{
    public interface IStatusEffectService : ITickable
    {
        void AddStatusEffect<T>(IDamageable target, T config) where T : StatusEffectConfig;
    }
}

