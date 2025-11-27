using System;
using BoleteHell.Code.Audio;
using BoleteHell.Code.Graphics;
using BoleteHell.Code.Utils;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace BoleteHell.Code.Gameplay.Damage.Effects.Impl
{
    using IStatusEffectTarget = GameObject;
    [Serializable]
    public sealed class PoisonStatusEffectConfig : StatusEffectConfig
    {
        [MinValue(1)]
        public int DamageEachTick = 10;

        protected override StatusEffectComparison Compare(StatusEffectConfig other)
        { 
            if (other is not PoisonStatusEffectConfig otherPoison) 
                return StatusEffectComparison.CannotCompare;

            var potency = DamageEachTick * duration;
            var otherPotency = otherPoison.DamageEachTick * otherPoison.duration;
            
            if (potency > otherPotency)
                return StatusEffectComparison.Stronger;
            
            if (potency < otherPotency)
                return StatusEffectComparison.Weaker;
            
            return StatusEffectComparison.Equal;
        }
    }
    
    public sealed class PoisonStatusEffect : IStatusEffect<PoisonStatusEffectConfig>
    {
        [Inject]
        private TransientLight.Pool _transientLightPool;
        
        [Inject]
        private IAudioPlayer _audioPlayer;

        public bool CanApply(IStatusEffectTarget target, PoisonStatusEffectConfig config)
        {
            return target.HasComponent<HealthComponent>();
        }

        public void Apply(IStatusEffectTarget target, PoisonStatusEffectConfig config)
        {
            var healthComp = target.GetComponent<HealthComponent>();
            healthComp.TakeDamage(config.DamageEachTick);
            
            Vector3 pos = target.transform.position;
            _audioPlayer.PlaySoundAsync("sfx_poison", pos);
            _transientLightPool.Spawn(pos, 1.2f, 0.5f);
        }

        public void Unapply(IStatusEffectTarget target, PoisonStatusEffectConfig config)
        {
            var healthComp = target.GetComponent<HealthComponent>();
            healthComp.TakeDamage(-config.DamageEachTick);
        }
    }
}