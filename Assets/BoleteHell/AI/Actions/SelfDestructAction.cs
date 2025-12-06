using System;
using System.Collections.Generic;
using BoleteHell.Code.Core;
using BoleteHell.Code.Graphics;
using BoleteHell.Gameplay.Characters;
using BoleteHell.Rendering.Ripples;
using BoleteHell.Utils;
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

        private MainRenderer _rendererRef;
        private MaterialPropertyBlock _propertyBlock;
        private Color _originalColor;
        private float _countdown;
        
        private static readonly int _colorId = Shader.PropertyToID("_Color");
    
        private TransientLight.Pool _explosionVFXPool;
        private RippleManager _rippleManager;

        protected override Status OnStart()
        {
            ServiceLocator.Get(out _explosionVFXPool);
            ServiceLocator.Get(out _rippleManager);
        
            GameObject.GetComponentChecked(out _rendererRef);
            
            _propertyBlock = new MaterialPropertyBlock();
            _rendererRef.Renderer.GetPropertyBlock(_propertyBlock);
            _originalColor = _propertyBlock.GetColor(_colorId);
            if (_originalColor == Color.clear)
                _originalColor = _rendererRef.Renderer.sharedMaterial.color;
            
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
            
            if (GameObject.TryGetComponent(out HealthComponent health))
            {
                health.TakeDamage(health.CurrentHealth);
            }
            else
            {
                Object.Destroy(GameObject);
            }
        
            return Status.Success;
        }
    
        private void FlashColor()
        {
            float t = Mathf.Clamp01(1f - (_countdown / TimeBeforeExplosion));
            float flashFrequency = Mathf.Lerp(0.8f, 0.1f, t);
            float flashPhase = Mathf.PingPong(Time.time * (1f / flashFrequency), 1f);

            Color flashColor = Color.Lerp(_originalColor, Color.red, flashPhase);
            _propertyBlock.SetColor(_colorId, flashColor);
            _rendererRef.Renderer.SetPropertyBlock(_propertyBlock);
        }
    
        private void Explode()
        {
            Vector2 position = GameObject.transform.position;
            
            DrawVisuals(position);
            _rippleManager?.EmitRipple(position, 10f);
            
            var filter = new ContactFilter2D();
            filter.SetLayerMask(~0);
            
            var results = new List<Collider2D>();
            int hitCollidersAmount = Physics2D.OverlapCircle(position, ExplosionRadius, filter, results);
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
            if (!ExplosionObject.Value)
            {
                Debug.LogWarning("Explosion hit missing its explosion visual effect");
                return;
            }
            
            _explosionVFXPool.Spawn(hitPosition, ExplosionRadius, 0.1f);
        }
    }
}

