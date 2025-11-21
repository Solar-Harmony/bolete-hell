using BoleteHell.Code.Core;
using BoleteHell.Code.Gameplay.Damage;
using BoleteHell.Code.Gameplay.Damage.Effects;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BoleteHell.Code.Arsenal.Rays.RayLogic
{
    public class StatusEffectOnHit : RayHitLogic
    {
        [SerializeReference] [Required]
        private StatusEffectConfig statusEffectConfig;

        private IStatusEffectService _statusEffectService;
        
        public override void OnHitImpl(Vector2 hitPosition, IDamageable victim, LaserInstance laser)
        {
            ServiceLocator.Get(ref _statusEffectService);
            
            if (victim is IStatusEffectTarget target)
            {
                _statusEffectService.AddStatusEffect(target, statusEffectConfig);
            }
        }
    }
}
