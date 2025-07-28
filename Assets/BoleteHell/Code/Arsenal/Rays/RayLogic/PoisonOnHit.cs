using System.Collections;
using BoleteHell.Code.Character;
using BoleteHell.Code.Gameplay.Health;
using BoleteHell.Code.Utils;
using UnityEngine;
using Zenject;

namespace BoleteHell.Code.Arsenal.Rays.RayLogic
{
    // TODO: As of now, effect can stack. We need a status effect system to handle effects on characters properly.
    public class PoisonOnHit : RayHitLogic
    {
        [SerializeField] private float poisonDuration = 5.0f;
        [SerializeField] private int poisonDamage = 2;
        [SerializeField] [Min(0.5f)] private float poisonTickInterval = 1.0f;

        [Inject]
        private IGlobalCoroutine _coroutine;
        
        public override void OnHitImpl(Vector2 hitPosition, IHealth hitCharacterHealth)
        {
            hitCharacterHealth.TakeDamage(baseHitDamage);
            _coroutine.Launch(PoisonEffect(hitCharacterHealth));
        }
        
        private IEnumerator PoisonEffect(IHealth hitCharacterHealth)
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