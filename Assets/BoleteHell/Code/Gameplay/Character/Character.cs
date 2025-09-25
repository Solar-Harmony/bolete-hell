using System;
using System.Collections.Generic;
using BoleteHell.Code.Arsenal.HitHandler;
using BoleteHell.Code.Arsenal.RayData;
using BoleteHell.Code.Gameplay.Damage;
using BoleteHell.Code.Gameplay.Damage.Effects;
using BoleteHell.Code.Gameplay.Destructible;
using BoleteHell.Code.Graphics;
using UnityEngine;
using Zenject;

namespace BoleteHell.Code.Gameplay.Character
{
    [RequireComponent(typeof(Health))]
    public abstract class Character : MonoBehaviour, ITargetable, IMovable, ISceneObject, IStatusEffectTarget, IDamageDealer, IFaction
    {
        public Health Health { get; private set; }

        public Vector2 Position => transform.position;
        
        public bool IsValid => this && gameObject;
        
        [field: SerializeField]
        public float MovementSpeed { get; set; } = 5f;

        [field: SerializeField]
        public float GeneralDamageMultiplier { get; set; } = 1f;

        public Dictionary<Faction, float> factionDamageMultiplier { get; set; } = new ();

        [field: SerializeField]
        public Energy Energy { get; private set; }

        public abstract Faction faction { get; set; }

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
        
        public virtual void OnHit(ITargetable.Context ctx, Action<ITargetable.Response> callback = null)
        {
            if (ctx.Data is not LaserCombo laser)
            {
                // TODO: make/find a filtered logging system
                Debug.LogWarning($"Hit data is not a CombinedLaser. Ignored hit.");
                return;
            }
            
            if (!((IFaction)this).IsAffected(laser.HitSide, ctx.Instigator))
                return;
            
            _explosionVFXPool.Spawn(ctx.Position, 0.5f, 0.1f);
        
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