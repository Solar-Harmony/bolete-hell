namespace BoleteHell.Code.Gameplay.Damage.Effects
{
    public interface IStatusEffect
    {
        bool CanApply(IDamageable target, StatusEffectConfig config);
        void Apply(IDamageable target, StatusEffectConfig config);
    }
    
    public interface IStatusEffect<in T> : IStatusEffect where T : StatusEffectConfig
    {
        void Apply(IDamageable target, T config);
        void IStatusEffect.Apply(IDamageable target, StatusEffectConfig config) => Apply(target, (T)config);
        bool IStatusEffect.CanApply(IDamageable target, StatusEffectConfig config) => config is T;
    }
}