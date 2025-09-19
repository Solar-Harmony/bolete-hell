using System;
using BoleteHell.Code.Arsenal.HitHandler;
using BoleteHell.Code.Arsenal.RayData;
using BoleteHell.Code.Gameplay.Damage;
using BoleteHell.Code.Gameplay.Destructible;
using BoleteHell.Code.Graphics;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;

namespace BoleteHell.Code.Gameplay.Character
{
    [RequireComponent(typeof(Health))]
    public abstract class Character : MonoBehaviour, ITargetable, ISceneObject
    {
        public Health Health { get; private set; }

        public Vector2 Position => transform.position;

        [SerializeField]
        private SpriteFragmentConfig spriteFragmentConfig;
        
        [field: SerializeField]
        private GameObject hitFeedbackEffect;
        
        [Inject]
        private ISpriteFragmenter _spriteFragmenter;
        
        [Inject]
        private TransientLight.Pool _explosionVFXPool;

        private ParticleSystem _fire;
        
        protected virtual void Awake()
        {
            Health = GetComponent<Health>();
            Health.OnDeath += () =>
            {
                _spriteFragmenter.Fragment(transform, spriteFragmentConfig);
                gameObject.SetActive(false);
                Destroy(gameObject);
            };
            _fire = GetComponentInChildren<ParticleSystem>();
        }
        
        public void OnHit(ITargetable.Context ctx, Action<ITargetable.Response> callback = null)
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
        
            laser.CombinedEffect(ctx.Position, this);
            callback?.Invoke(new ITargetable.Response(ctx){ RequestDestroyProjectile = true });

            if (_fire)
            {
                ParticleSystem.MainModule mainModule = _fire.main;
                float alpha =  1 - (Health.CurrentHealth / (float)Health.MaxHealth);
                mainModule.startColor = _fire.main.startColor.color.WithAlpha(alpha);
            }
        }
    }
}