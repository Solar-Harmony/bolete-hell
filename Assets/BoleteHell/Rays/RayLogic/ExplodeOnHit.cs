using System;
using System.Collections.Generic;
using BoleteHell.Utils;
using Lasers.RayLogic;
using UnityEngine;

namespace BoleteHell.Rays.RayLogic
{
    [Serializable]
    public class ExplodeOnHit : RayHitLogic
    {
        
        [SerializeField] private int explosionDamage;
        
        [SerializeField] private float explosionRadius;

        [SerializeField] private GameObject explosionCircle;
        
        //Peut-être pouvoir déterminer si l'explosion affecte le joueur et les ennemis ou seulement les ennemis
        

        public override void OnHit(Vector2 hitPosition,Health hitCharacterHealth)
        {
            hitCharacterHealth.TakeDamage(baseHitDamage);
            
            ContactFilter2D filter = new ContactFilter2D();
            filter.SetLayerMask(LayerMask.GetMask("Unit"));
            List<Collider2D> results = new List<Collider2D>();

            int hitCollidersAmount = Physics2D.OverlapCircle(hitPosition, explosionRadius,filter,results);
            
            if (hitCollidersAmount <= 0)
            {
                Debug.Log("Explosion hit nothing");
                return;
            }
            
            for (int i = 0; i < hitCollidersAmount; i++)
            {
                Collider2D hit = results[i];
                if (hit.gameObject.TryGetComponent(out Health health))
                {
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

            explosionCircle.transform.localScale = new Vector3(explosionRadius*2, explosionRadius*2, 1);
            ObjectInstantiator.InstantiateObjectForAmountOfTime(explosionCircle,hitPosition,0.1f);
        }
    }
}