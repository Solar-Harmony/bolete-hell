using System;
using System.Collections.Generic;
using BoleteHell.Code.Arsenal.HitHandler;
using BoleteHell.Code.Arsenal.RayData;
using BoleteHell.Code.Gameplay.Damage;
using BoleteHell.Code.Gameplay.Damage.Effects;
using BoleteHell.Code.Graphics;
using UnityEngine;
using Zenject;

namespace BoleteHell.Code.Gameplay.Characters
{
    [RequireComponent(typeof(Health))]
    public abstract class Character : MonoBehaviour, ITargetable, IMovable, ISceneObject, IStatusEffectTarget, IDamageDealer, IFaction, IInstigator
    {
        public Health Health { get; private set; }
        public GameObject GameObject => gameObject;

        public Vector2 Position => transform.position;
        
        public bool IsValid => this && gameObject;
        
        [field: SerializeField]
        public float MovementSpeed { get; set; } = 5f;

        [field: SerializeField]
        public float GeneralDamageMultiplier { get; set; } = 1f;

        public Dictionary<FactionType, float> factionDamageMultiplier { get; set; } = new ();

        [field: SerializeField]
        public Energy Energy { get; private set; }

        public abstract FactionType faction { get; set; }
        
        [field: SerializeField]
        private GameObject hitFeedbackEffect;
        
        [Inject]
        private TransientLight.Pool _explosionVFXPool;

        private ParticleSystem _fire;
        
        protected virtual void Awake()
        {
            Health = GetComponent<Health>();
            _fire = GetComponentInChildren<ParticleSystem>();
        }
        
        public virtual void OnHit(ITargetable.Context ctx, Action<ITargetable.Response> callback = null)
        {
            if (ctx.Data is not LaserCombo laser)
            {
                Debug.LogWarning($"Hit data is not a CombinedLaser. Ignored hit.");
                return;
            }

            bool isAffected = ((IFaction)this).IsAffected(ctx.Projectile.AffectedSide, ctx.Instigator);
            if (!isAffected)
            {
                //Si la personne n'est pas affecter on laisse toujours le laser passer
                callback?.Invoke(new ITargetable.Response(ctx));
                return;
            }

            ProcessHitEffects(ctx, laser);
            
            callback?.Invoke(new ITargetable.Response(ctx){RequestDestroyProjectile = ctx.Projectile.isProjectile});
        }

        private void ProcessHitEffects(ITargetable.Context ctx, LaserCombo laser)
        {
            _explosionVFXPool.Spawn(ctx.Position, 0.5f, 0.1f);
            laser.CombinedEffect(ctx.Position, this, ctx.Projectile);

            if (!_fire) 
                return;
            
            ParticleSystem.MainModule mainModule = _fire.main;
            float alpha =  1 - (Health.CurrentHealth / (float)Health.MaxHealth);
            var color = _fire.main.startColor.color;
            color.a = alpha;
            mainModule.startColor = color;
        }
    }
}