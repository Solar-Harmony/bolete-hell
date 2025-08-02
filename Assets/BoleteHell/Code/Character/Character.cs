using System;
using BoleteHell.Code.Arsenal.HitHandler;
using BoleteHell.Code.Arsenal.RayData;
using BoleteHell.Code.Gameplay.Destructible;
using BoleteHell.Code.Gameplay.Health;
using BoleteHell.Code.Graphics;
using UnityEngine;
using Zenject;

namespace BoleteHell.Code.Character
{
    public abstract class Character : MonoBehaviour, IHitHandler
    {
        [SerializeField]
        public Health health;
        
        [SerializeField]
        private SpriteFragmentConfig spriteFragmentConfig;
        
        [field: SerializeField]
        private GameObject hitFeedbackEffect;
        
        [Inject]
        private ISpriteFragmenter _spriteFragmenter;
        
        [Inject]
        private TransientLight.Pool _explosionVFXPool;
        
        protected virtual void Awake()
        {
            health.OnDeath += () =>
            {
                _spriteFragmenter.Fragment(transform, spriteFragmentConfig);
                gameObject.SetActive(false);
                Destroy(gameObject);
            };
        }
        
        public void OnHit(IHitHandler.Context ctx, Action<IHitHandler.Response> callback = null)
        {
            // TODO: make a proper factions system
            if (ctx.Instigator && ctx.Instigator.gameObject.CompareTag(gameObject.tag))
                return;
            
            _explosionVFXPool.Spawn(ctx.Position, 0.5f, 0.1f);
            
            if (ctx.Data is not LaserCombo laser)
            {
                // TODO: make/find a filtered logging system
                Debug.LogWarning($"Hit data is not a CombinedLaser. Ignored hit.");
                return;
            }
        
            laser.CombinedEffect(ctx.Position, health); // TODO: Should we really pass health here
            callback?.Invoke(new IHitHandler.Response(ctx){ RequestDestroy = true });
        }
    }
}