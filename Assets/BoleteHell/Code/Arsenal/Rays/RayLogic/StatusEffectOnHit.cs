using BoleteHell.Code.Gameplay.Damage;
using BoleteHell.Code.Gameplay.Damage.Effects;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace BoleteHell.Code.Arsenal.Rays.RayLogic
{
    public class StatusEffectOnHit : RayHitLogic
    {
        [SerializeReference] [Required]
        private StatusEffectConfig statusEffectConfig;

        [Inject]
        private IStatusEffectService _statusEffectService;
        
        public override void OnHitImpl(Vector2 hitPosition, IDamageable hitCharacterHealth)
        {
            hitCharacterHealth.Health.TakeDamage(baseHitDamage);
            _statusEffectService.AddStatusEffect(hitCharacterHealth, statusEffectConfig);
        }
    }
}
