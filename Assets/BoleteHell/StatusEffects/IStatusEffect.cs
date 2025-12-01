using System;
using UnityEngine;

namespace BoleteHell.Code.Gameplay.Damage.Effects
{
    public interface IStatusEffect
    {
        Type ConfigType { get; }
        
        bool CanApply(GameObject target, StatusEffectConfig config);
        void Apply(GameObject target, StatusEffectConfig config);
        void Unapply(GameObject target, StatusEffectConfig config);
    }
    
    public interface IStatusEffect<in T> : IStatusEffect where T : StatusEffectConfig
    {
        Type IStatusEffect.ConfigType => typeof(T);
        
        bool CanApply(GameObject target, T config);
        void Apply(GameObject target, T config);
        void Unapply(GameObject target, T config);
        
        bool IStatusEffect.CanApply(GameObject target, StatusEffectConfig config) => CanApply(target, (T)config);
        void IStatusEffect.Apply(GameObject target, StatusEffectConfig config) => Apply(target, (T)config);
        void IStatusEffect.Unapply(GameObject target, StatusEffectConfig config) => Unapply(target, (T)config);
    }
}