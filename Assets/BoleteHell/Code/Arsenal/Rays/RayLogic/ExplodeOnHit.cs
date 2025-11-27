using System;
using System.Collections.Generic;
using BoleteHell.Code.Core;
using BoleteHell.Code.Gameplay.Characters;
using BoleteHell.Code.Gameplay.Damage;
using BoleteHell.Code.Graphics;
using BoleteHell.Code.Utils.LogFilter;
using UnityEngine;

namespace BoleteHell.Code.Arsenal.Rays.RayLogic
{
    [Serializable]
    public class ExplodeOnHit : RayHitLogic
    {
        [SerializeField] private int explosionDamage;
        [SerializeField] private float explosionRadius;
        [SerializeField] private GameObject explosionCircle;
 
        private TransientLight.Pool _explosionVFXPool;

        public override void OnHitImpl(Vector2 hitPosition, HealthComponent victim, LaserInstance laser)
        {
            ServiceLocator.Get(out _explosionVFXPool);

            DrawVisuals(hitPosition);
            
            var filter = new ContactFilter2D();
            filter.SetLayerMask(LayerMask.GetMask("Unit", "PlayerEnemy"));

            var hits = new List<Collider2D>();
            Physics2D.OverlapCircle(hitPosition, explosionRadius, filter, hits);
            if (hits.Count == 0)
                return;

            foreach (Collider2D hit in hits)
            {
                if (!hit.TryGetComponent(out HealthComponent health))
                    continue;
                
                bool hasFaction = hit.TryGetComponent(out FactionComponent faction);
                bool hasInstigatorFaction = laser.Instigator.TryGetComponent(out FactionComponent instigatorFaction);
                bool isAffected = hasFaction && hasInstigatorFaction && faction.IsAffected(laser.AffectedSide, instigatorFaction);
                if (!isAffected)
                    continue;
                
                DamageDealerComponent instigator = laser.Instigator.GetComponent<DamageDealerComponent>();
                int damage = ComputeActualDamage(explosionDamage, health.gameObject, instigator);
                
                Scribe.Log(LogHits, "{0} was hit by {1}'s {2}hp explosion and lost {3}hp.",
                    hit.gameObject.name,
                    laser.Instigator.name,
                    explosionDamage,
                    damage);
                    
                health.TakeDamage(damage);
            }
        }

        private void DrawVisuals(Vector2 hitPosition)
        {
            if (!explosionCircle)
            {
                Debug.LogWarning("Explosion hit missing its explosion visual effect");
                return;
            }
         
            _explosionVFXPool.Spawn(hitPosition, explosionRadius, 0.1f);
        }
    }
}