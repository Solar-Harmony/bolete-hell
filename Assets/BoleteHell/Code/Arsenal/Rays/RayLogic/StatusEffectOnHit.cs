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
        
        public override void OnHitImpl(Vector2 hitPosition, HealthComponent victim, LaserInstance laser)
        {
            ServiceLocator.Get(out _statusEffectService);
            _statusEffectService.AddStatusEffect(victim.gameObject, statusEffectConfig);
        }
    }
}
