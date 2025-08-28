using System;

namespace BoleteHell.Code.Gameplay.Damage.Effects
{
    public interface IStatusEffect
    {
        Type ConfigType { get; }
        
        bool CanApply(IStatusEffectTarget target, StatusEffectConfig config);
        void Apply(IStatusEffectTarget target, StatusEffectConfig config);
        void Unapply(IStatusEffectTarget target, StatusEffectConfig config);
    }
    
    public interface IStatusEffect<in T> : IStatusEffect where T : StatusEffectConfig
    {
        Type IStatusEffect.ConfigType => typeof(T);
        
        bool CanApply(IStatusEffectTarget target, T config);
        void Apply(IStatusEffectTarget target, T config);
        void Unapply(IStatusEffectTarget target, T config);
        
        bool IStatusEffect.CanApply(IStatusEffectTarget target, StatusEffectConfig config) => CanApply(target, (T)config);
        void IStatusEffect.Apply(IStatusEffectTarget target, StatusEffectConfig config) => Apply(target, (T)config);
        void IStatusEffect.Unapply(IStatusEffectTarget target, StatusEffectConfig config) => Unapply(target, (T)config);
    }
}