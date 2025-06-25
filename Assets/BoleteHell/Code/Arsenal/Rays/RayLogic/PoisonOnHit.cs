using System.Collections;
using _BoleteHell.Code.Character;
using _BoleteHell.Code.ProjectileSystem.Rays.RayLogic;
using _BoleteHell.Code.Utils;
using UnityEngine;

namespace Lasers.RayLogic
{
    // TODO: As of now, effect can stack. We need a status effect system to handle effects on characters properly.
    public class PoisonOnHit : RayHitLogic
    {
        [SerializeField] private float poisonDuration = 5.0f;
        [SerializeField] private int poisonDamage = 2;
        [SerializeField] [Min(0.5f)] private float poisonTickInterval = 1.0f;
        
        public override void OnHit(Vector2 hitPosition, Health hitCharacterHealth)
        {
            hitCharacterHealth.TakeDamage(baseHitDamage);
            GlobalCoroutine.Launch(PoisonEffect(hitCharacterHealth));
        }
        
        private IEnumerator PoisonEffect(Health hitCharacterHealth)
        {
            float elapsedTime = 0f;
            while (elapsedTime < poisonDuration)
            {
                hitCharacterHealth.TakeDamage(poisonDamage);
                elapsedTime += 1f;
                yield return new WaitForSeconds(poisonTickInterval);
            }
        }
    }
}