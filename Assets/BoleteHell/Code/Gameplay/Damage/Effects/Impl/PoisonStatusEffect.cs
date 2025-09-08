using System;
using BoleteHell.Code.Audio;
using BoleteHell.Code.Gameplay.Character;
using BoleteHell.Code.Graphics;
using Sirenix.OdinInspector;
using Zenject;

namespace BoleteHell.Code.Gameplay.Damage.Effects.Impl
{
    [Serializable]
    public sealed class PoisonStatusEffectConfig : StatusEffectConfig
    {
        [MinValue(1)]
        public int damageEachTick = 10;

        protected override StatusEffectComparison Compare(StatusEffectConfig other)
        {
            if (other is not PoisonStatusEffectConfig otherPoison) 
                return StatusEffectComparison.CannotCompare;

            var potency = damageEachTick * duration;
            var otherPotency = otherPoison.damageEachTick * otherPoison.duration;
            
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
            return target is IDamageable;
        }

        public void Apply(IStatusEffectTarget target, PoisonStatusEffectConfig config)
        {
            if (target is IDamageable damageable)
            {
                damageable.Health.TakeDamage(config.damageEachTick);
            }

            if (target is ISceneObject sceneObject) 
            {
                _audioPlayer.PlaySoundAsync("sfx_poison", sceneObject.Position);
                _transientLightPool.Spawn(sceneObject.Position, 1.2f, 0.5f);
            }
        }

        public void Unapply(IStatusEffectTarget target, PoisonStatusEffectConfig config)
        {
            if (target is IDamageable damageable)
            {
                damageable.Health.TakeDamage(-config.damageEachTick);
            }
        }
    }
}