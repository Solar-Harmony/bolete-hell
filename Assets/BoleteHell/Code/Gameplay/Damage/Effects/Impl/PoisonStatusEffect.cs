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
    }
    
    public sealed class PoisonStatusEffect : IStatusEffect<PoisonStatusEffectConfig>
    {
        [Inject]
        private TransientLight.Pool _transientLightPool;
        
        [Inject]
        private IAudioPlayer _audioPlayer;
        
        public void Apply(object target, PoisonStatusEffectConfig config)
        {
            if (target is IDamageable damageable)
            {
                damageable.Health.TakeDamage(config.damageEachTick);
            }

            if (target is ISceneObject sceneObject)
            {
                _audioPlayer.PlaySoundAsync("sfx_poison", sceneObject.Position);
                // TODO: Support custom colors
                _transientLightPool.Spawn(sceneObject.Position, 1.2f, 0.5f);
            }
        }
    }
}