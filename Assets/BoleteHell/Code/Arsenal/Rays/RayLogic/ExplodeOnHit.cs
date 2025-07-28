using System;
using System.Collections.Generic;
using BoleteHell.Code.Character;
using BoleteHell.Code.Gameplay.Health;
using BoleteHell.Code.Graphics;
using BoleteHell.Code.Utils;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Zenject;

namespace BoleteHell.Code.Arsenal.Rays.RayLogic
{
    [Serializable]
    public class ExplodeOnHit : RayHitLogic, IRequestManualInject
    {
        [SerializeField] private int explosionDamage;
        
        [SerializeField] private float explosionRadius;

        [SerializeField] private GameObject explosionCircle;
 
        [Inject]
        private TransientLight.Pool _explosionVFXPool;

        //Peut-être pouvoir déterminer si l'explosion affecte le joueur et les ennemis ou seulement les ennemis
        public override void OnHit(Vector2 hitPosition, IHealth hitCharacterHealth)
        {
            ((IRequestManualInject)this).InjectDependencies();
            
            hitCharacterHealth.TakeDamage(baseHitDamage);
            
            ContactFilter2D filter = new ContactFilter2D();
            filter.SetLayerMask(LayerMask.GetMask("Unit"));
            List<Collider2D> results = new List<Collider2D>();

            int hitCollidersAmount = Physics2D.OverlapCircle(hitPosition, explosionRadius,filter,results);
            
            if (hitCollidersAmount <= 0)
            {
                Debug.Log("Explosion hit nothing");
            }
            else
            {
                for (int i = 0; i < hitCollidersAmount; i++)
                {
                    Collider2D hit = results[i];
                    if (!hit.gameObject.TryGetComponent(out Health health)) continue;
                    
                    Debug.Log($"Explosion hit {hit.name}",hit.gameObject);
                    health.TakeDamage(explosionDamage);
                }
            }
            DrawVisuals(hitPosition);
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

        bool IRequestManualInject.IsInjected { get; set; } = false;
    }
}