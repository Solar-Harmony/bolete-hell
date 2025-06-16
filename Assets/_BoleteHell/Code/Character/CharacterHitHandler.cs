using _BoleteHell.Code.Character;
using _BoleteHell.Code.ProjectileSystem.HitHandler;
using Data.Rays;
using UnityEngine;

namespace _BoleteHell.Code.Player
{
    [RequireComponent(typeof(Health))]
    public class CharacterHitHandler : MonoBehaviour, IHitHandler
    {
        public void OnHit(IHitHandler.Context ctx)
        {
            if (ctx.Data is not CombinedLaser laser)
            {
                Debug.LogWarning($"Hit data is not a CombinedLaser. Ignored hit.");
                return;
            }
        
            Health health = GetComponent<Health>();
            laser.CombinedEffect(ctx.Position, health);
        }
    }
}