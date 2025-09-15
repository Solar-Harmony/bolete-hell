using BoleteHell.Code.Audio.BoleteHell.Models;
using BoleteHell.Gameplay.Damage.Effects;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace BoleteHell.Gameplay.Lasers
{
    public class StatusEffectOnHit : RayHitLogic
    {
        [SerializeReference] [Required]
        private StatusEffectConfig statusEffectConfig;

        [Inject]
        private IStatusEffectService _statusEffectService;
        
        public override void OnHitImpl(Vector2 hitPosition, IDamageable victim)
        {
            if (victim is IStatusEffectTarget target)
            {
                _statusEffectService.AddStatusEffect(target, statusEffectConfig);
            }
        }
    }
}
