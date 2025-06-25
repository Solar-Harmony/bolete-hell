using System;
using BoleteHell.Code.Arsenal.HitHandler;
using BoleteHell.Code.Arsenal.RayData;
using BoleteHell.Code.Utils;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace BoleteHell.Code.Character
{
    [RequireComponent(typeof(Health))]
    public class CharacterHitHandler : MonoBehaviour, IHitHandler
    {
        public bool isInvincible = false;
        public GameObject explosionCircle;

        public void OnHit(IHitHandler.Context ctx, Action<IHitHandler.Response> callback = null)
        {
            if (isInvincible)
                return;

            // TODO: make a proper factions system
            if (ctx.Instigator && ctx.Instigator.gameObject.CompareTag(gameObject.tag))
                return;
            
            if (explosionCircle.TryGetComponent(out Light2D light2D))
            {
                light2D.pointLightOuterRadius = 0.5f;
                ObjectInstantiator.InstantiateObjectForAmountOfTime(explosionCircle, ctx.Position, 0.1f);
            }
            
            if (ctx.Data is not LaserCombo laser)
            {
                Debug.LogWarning($"Hit data is not a CombinedLaser. Ignored hit.");
                return;
            }
        
            Health health = GetComponent<Health>();
            laser.CombinedEffect(ctx.Position, health);
            callback?.Invoke(new IHitHandler.Response(ctx));
        }
    }
}