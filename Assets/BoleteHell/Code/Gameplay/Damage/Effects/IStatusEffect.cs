namespace BoleteHell.Code.Gameplay.Damage.Effects
{
    public interface IStatusEffect
    {
        bool CanApply(object target, StatusEffectConfig config);
        void Apply(object target, StatusEffectConfig config);
    }
    
    public interface IStatusEffect<in T> : IStatusEffect where T : StatusEffectConfig
    {
        void Apply(object target, T config);
        void IStatusEffect.Apply(object target, StatusEffectConfig config) => Apply(target, (T)config);
        bool IStatusEffect.CanApply(object target, StatusEffectConfig config) => config is T;
    }
    
    public interface ITransientStatusEffect : IStatusEffect
    {
        void Unapply(object target, StatusEffectConfig config);
    }
    
    public interface ITransientStatusEffect<in T> : ITransientStatusEffect where T : StatusEffectConfig
    {
        bool IStatusEffect.CanApply(object target, StatusEffectConfig config) => config is T;
        void IStatusEffect.Apply(object target, StatusEffectConfig config) => Apply(target, (T)config);
        void Apply(object target, T config);
        void Unapply(object target, T config);
        void ITransientStatusEffect.Unapply(object target, StatusEffectConfig config) => Unapply(target, (T)config);
    }
}