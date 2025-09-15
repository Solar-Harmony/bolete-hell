using System;
using System.Collections.Generic;
using BoleteHell.Code.Audio.BoleteHell.Models;
using BoleteHell.Gameplay.Graphics;
using UnityEngine;
using Zenject;

namespace BoleteHell.Gameplay.Lasers
{
    [Serializable]
    public class ExplodeOnHit : RayHitLogic
    {
        [SerializeField] private int explosionDamage;
        
        [SerializeField] private float explosionRadius;

        [SerializeField] private GameObject explosionCircle;
 
        [Inject]
        private TransientLight.Pool _explosionVFXPool;

        //Peut-être pouvoir déterminer si l'explosion affecte le joueur et les ennemis ou seulement les ennemis
        public override void OnHitImpl(Vector2 hitPosition, IDamageable hitCharacterHealth)
        {
            ContactFilter2D filter = new ContactFilter2D();
            filter.SetLayerMask(LayerMask.GetMask("Unit"));
            List<Collider2D> results = new List<Collider2D>();
            Debug.Log($"hit {hitPosition}");
            DrawVisuals(hitPosition);
            
            int hitCollidersAmount = Physics2D.OverlapCircle(hitPosition, explosionRadius, filter, results);
            if (hitCollidersAmount <= 0)
                return;

            for (int i = 0; i < hitCollidersAmount; i++)
            {
                Collider2D hit = results[i];
                if (!hit.gameObject.TryGetComponent(out Character.Character character)) 
                    continue;
                    
                character.Health.TakeDamage(explosionDamage);
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