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

        public override void OnHitImpl(Vector2 hitPosition, IDamageable victim, LaserInstance laser)
        {
            ServiceLocator.Get(ref _explosionVFXPool);

            DrawVisuals(hitPosition);
            
            var filter = new ContactFilter2D();
            filter.SetLayerMask(LayerMask.GetMask("Unit", "PlayerEnemy"));

            var hits = new List<Collider2D>();
            Physics2D.OverlapCircle(hitPosition, explosionRadius, filter, hits);
            if (hits.Count == 0)
                return;

            foreach (Collider2D hit in hits)
            {
                if (!hit.gameObject.TryGetComponent(out Character character)) 
                    continue;

                IFaction faction = character;
                bool isAffected = faction.IsAffected(laser.AffectedSide, laser.Instigator);
                if (!isAffected)
                    continue;
                
                int damage = ComputeActualDamage(explosionDamage, character, laser.Instigator);
                
                Scribe.Log(LogHits, "{0} was hit by {1}'s {2}hp explosion and lost {3}hp.",
                    character.GameObject.name,
                    laser.Instigator.GameObject.name,
                    explosionDamage,
                    damage);
                    
                character.Health.TakeDamage(damage);
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