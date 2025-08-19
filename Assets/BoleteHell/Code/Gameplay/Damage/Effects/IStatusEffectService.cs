namespace BoleteHell.Code.Gameplay.Damage.Effects
{
    public interface IStatusEffectService
    {
        void AddStatusEffect<T>(IDamageable target, T config) where T : StatusEffectConfig;
    }
}

