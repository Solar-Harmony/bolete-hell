using System;
using BoleteHell.Arsenals.HitHandler;
using BoleteHell.Arsenals.LaserData;
using BoleteHell.Code.Audio.BoleteHell.Models;
using BoleteHell.Gameplay.Damage.Effects;
using BoleteHell.Gameplay.Destructible;
using BoleteHell.Gameplay.Graphics;
using BoleteHell.Models;
using UnityEngine;
using Zenject;

namespace BoleteHell.Gameplay.Character
{
    public abstract class Character : MonoBehaviour, ITargetable, IMovable, ISceneObject, IStatusEffectTarget, IDamageDealer
    {
        [field: SerializeField]
        public Health Health { get; set; }
        
        public Vector2 Position => transform.position;
        
        public bool IsValid => this && gameObject;
        
        [field: SerializeField]
        public float MovementSpeed { get; set; } = 5f;

        [field: SerializeField]
        public float DamageMultiplier { get; set; } = 1f;

        [field: SerializeField]
        public Energy Energy { get; private set; }

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
        
            laser.CombinedEffect(ctx.Position, this, ctx.Projectile);
            callback?.Invoke(new ITargetable.Response(ctx){ RequestDestroyProjectile = true });

            if (_fire)
            {
                ParticleSystem.MainModule mainModule = _fire.main;
                float alpha =  1 - (Health.CurrentHealth / (float)Health.MaxHealth);
                var color = _fire.main.startColor.color;
                color.a = alpha;
                mainModule.startColor = color;
            }
        }

    }
}