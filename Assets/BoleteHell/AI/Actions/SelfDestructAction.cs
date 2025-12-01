using System;
using System.Collections.Generic;
using BoleteHell.Gameplay.Characters;
using BoleteHell.Utils.Extensions;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Object = UnityEngine.Object;

namespace BoleteHell.AI.Actions
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "StartSelfDestructSequence", story: "Self-destruct after [seconds] seconds", category: "Bolete Hell", id: "b0de2764a2db4b2a6af3d8c50adc5d83")]
    public partial class SelfDestructAction : Action
    {
        [SerializeReference] public BlackboardVariable<float> TimeBeforeExplosion;
        [SerializeReference] public BlackboardVariable<float> ExplosionRadius;
        [SerializeReference] public BlackboardVariable<int> ExplosionDamage;
        [SerializeReference] public BlackboardVariable<GameObject> ExplosionObject;

        private Renderer _renderer;
        private Color _originalColor;
        private float _countdown;
    
        // private TransientLight.Pool _explosionVFXPool;

        protected override Status OnStart()
        {
            // ServiceLocator.Get(out _explosionVFXPool);
        
            GameObject.GetComponentChecked(out _renderer);
            _originalColor = _renderer.material.color;
            _countdown = TimeBeforeExplosion;
        
            return Status.Running;
        }

        protected override Status OnUpdate()
        {
            _countdown -= Time.deltaTime;
        
            FlashColor();
        
            if (_countdown > 0) 
                return Status.Running;
        
            Explode();
            Object.Destroy(GameObject);
        
            return Status.Success;
        }
    
        private void FlashColor()
        {
            float t = Mathf.Clamp01(1f - (_countdown / TimeBeforeExplosion));
            float flashFrequency = Mathf.Lerp(0.8f, 0.1f, t);
            float flashPhase = Mathf.PingPong(Time.time * (1f / flashFrequency), 1f);

            _renderer.material.color = Color.Lerp(_originalColor, Color.red, flashPhase);
        }
    
        private void Explode()
        {
            DrawVisuals(GameObject.transform.position);
            
            var filter = new ContactFilter2D();
            filter.SetLayerMask(~0);
            
            var results = new List<Collider2D>();
            int hitCollidersAmount = Physics2D.OverlapCircle(GameObject.transform.position, ExplosionRadius, filter, results);
            if (hitCollidersAmount <= 0)
                return;

            for (int i = 0; i < hitCollidersAmount; i++)
            {
                Collider2D hit = results[i];
                if (!hit.gameObject.TryGetComponent(out HealthComponent health)) 
                    continue;
                    
                health.TakeDamage(ExplosionDamage);
            }
        }
    
        private void DrawVisuals(Vector2 hitPosition)
        {
            // trop la flemme
            // if (!ExplosionObject.Value)
            // {
            //     Debug.LogWarning("Explosion hit missing its explosion visual effect");
            //     return;
            // }
            //
            // _explosionVFXPool.Spawn(hitPosition, ExplosionRadius, 0.1f);
        }
    }
}

