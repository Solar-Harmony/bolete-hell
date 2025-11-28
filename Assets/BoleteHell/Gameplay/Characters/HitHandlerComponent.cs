using System;
using BoleteHell.Code.Arsenal.HitHandler;
using BoleteHell.Code.Arsenal.RayData;
using BoleteHell.Code.Graphics;
using UnityEngine;
using Zenject;

namespace BoleteHell.Gameplay.Characters
{
    [RequireComponent(typeof(HealthComponent), typeof(FactionComponent))]
    public class HitHandlerComponent : MonoBehaviour, ITargetable
    {
        [Inject]
        private TransientLight.Pool _explosionVFXPool;

        private FactionComponent _faction;
        private ParticleSystem _fire;
        protected HealthComponent _health;
        
        protected virtual void Awake()
        {
            _faction = GetComponent<FactionComponent>();
            _fire = GetComponentInChildren<ParticleSystem>();
            _health = GetComponent<HealthComponent>();
        }
        
        public virtual void OnHit(ITargetable.Context ctx, Action<ITargetable.Response> callback = null)
        {
            if (ctx.Data is not LaserCombo laser)
            {
                Debug.LogWarning($"Hit data is not a CombinedLaser. Ignored hit.");
                return;
            }

            bool otherIsFaction = ctx.Instigator.TryGetComponent<FactionComponent>(out var otherFaction);
            bool isAffected = otherIsFaction && _faction.IsAffected(ctx.Projectile.AffectedSide, otherFaction);
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
            _explosionVFXPool.Spawn(ctx.RayHit.point, 0.5f, 0.1f);
            laser.CombinedEffect(ctx.RayHit.point, _health, ctx.Projectile);

            if (!_fire) 
                return;
            
            ParticleSystem.MainModule mainModule = _fire.main;
            float alpha =  1 - (_health.CurrentHealth / (float)_health.MaxHealth);
            var color = _fire.main.startColor.color;
            color.a = alpha;
            mainModule.startColor = color;
        }
    }
}