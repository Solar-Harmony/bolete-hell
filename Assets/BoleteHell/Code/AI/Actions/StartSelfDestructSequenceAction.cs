using System;
using System.Collections.Generic;
using BoleteHell.Code.Core;
using BoleteHell.Code.Gameplay.Characters;
using BoleteHell.Code.Graphics;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using Zenject;
using Object = UnityEngine.Object;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "StartSelfDestructSequence", story: "[Self] destruct after [seconds] seconds", category: "Bolete Hell", id: "b0de2764a2db4b2a6af3d8c50adc5d83")]
public partial class StartSelfDestructSequenceAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<float> TimeBeforeExplosion;
    [SerializeReference] public BlackboardVariable<float> ExplosionRadius;
    [SerializeReference] public BlackboardVariable<int> ExplosionDamage;
    [SerializeReference] public BlackboardVariable<GameObject> ExplosionObject;

    private Renderer renderer;
    private Color originalColor;
    private bool startedCountdown;
    private float currentCountdownTimer = 0f;
    
    private float countdown;
    
    private TransientLight.Pool _explosionVFXPool;

    protected override Status OnStart()
    {
        ServiceLocator.Get(ref _explosionVFXPool);
        renderer = Self.Value.GetComponent<Renderer>();
        originalColor = renderer.material.color;
        countdown = TimeBeforeExplosion;
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        countdown -= Time.deltaTime;
        
        FlashColor();
        
        if (!(countdown <= 0)) return Status.Running;
        
        Explode();
        Object.Destroy(Self);
        
        return Status.Success;
    }
    
    private void FlashColor()
    {
        float t = Mathf.Clamp01(1f - (currentCountdownTimer / TimeBeforeExplosion));
        float flashFrequency = Mathf.Lerp(0.8f, 0.1f, t);
        float flashPhase = Mathf.PingPong(Time.time * (1f / flashFrequency), 1f);

        renderer.material.color = Color.Lerp(originalColor, Color.red, flashPhase);
    }
    
    private void Explode()
    {
        DrawVisuals(Self.Value.transform.position);
            
        var filter = new ContactFilter2D();
        filter.SetLayerMask(~0);
            
        var results = new List<Collider2D>();
        int hitCollidersAmount = Physics2D.OverlapCircle(Self.Value.transform.position, ExplosionRadius, filter, results);
        if (hitCollidersAmount <= 0)
            return;

        for (int i = 0; i < hitCollidersAmount; i++)
        {
            Collider2D hit = results[i];
            if (!hit.gameObject.TryGetComponent(out Character character)) 
                continue;
                    
            character.Health.TakeDamage(ExplosionDamage);
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

