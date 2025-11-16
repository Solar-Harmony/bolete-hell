using System.Collections;
using System.Collections.Generic;
using BoleteHell.Code.Core;
using BoleteHell.Code.Gameplay.Characters;
using BoleteHell.Code.Graphics;
using Pathfinding;
using UnityEngine;
using Zenject;

public class BomberAI : MonoBehaviour
{
    [SerializeField]
    private float selfDestroySequenceStartDistance = 5f;

    [SerializeField]
    private float timeBeforeExplosion = 3f;

    [SerializeField] private int explosionDamage;
    [SerializeField] private float explosionRadius;
    [SerializeField] private GameObject explosionCircle;

    private bool startedCountdown;
    private float currentCountdownTimer = 0f;

    [Inject]
    private TransientLight.Pool _explosionVFXPool;
    
    private Enemy _selfCharacter;
    private AIPath _pathfinder;
    private Renderer _renderer;

    private Color originalColor;

    private GameObject currentTarget;
    
    [Inject]
    private IEntityFinder _entities;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Hardcode pour juste focus le joueur pour l'instant
        currentTarget = _entities.GetPlayer().GameObject; 
        
        Debug.Assert(_pathfinder ??= gameObject.GetComponent<AIPath>());
        Debug.Assert(_selfCharacter ??= gameObject.GetComponent<Enemy>());
        Debug.Assert(_renderer ??= GetComponent<Renderer>());

        originalColor = _renderer.material.color;

        _pathfinder.maxSpeed = _selfCharacter.MovementSpeed;
        _pathfinder.destination = currentTarget.transform.position;
        _pathfinder.whenCloseToDestination = CloseToDestinationMode.Stop;
    }

    // Update is called once per frame
    void Update()
    {
        _pathfinder.destination = currentTarget.transform.position;

        if (!startedCountdown && _pathfinder.remainingDistance <= selfDestroySequenceStartDistance)
        {
            startedCountdown = true;
            currentCountdownTimer = timeBeforeExplosion;
        }

        if (!startedCountdown) return;
        
        currentCountdownTimer -= Time.deltaTime;

        FlashColor();

        if (!(currentCountdownTimer <= 0f)) return;
        
        Explode();
        Destroy(gameObject);
    }

    private void FlashColor()
    {
        float t = Mathf.Clamp01(1f - (currentCountdownTimer / timeBeforeExplosion));
        float flashFrequency = Mathf.Lerp(0.8f, 0.1f, t);
        float flashPhase = Mathf.PingPong(Time.time * (1f / flashFrequency), 1f);

        _renderer.material.color = Color.Lerp(originalColor, Color.red, flashPhase);
    }

    private void Explode()
    {
        DrawVisuals(transform.position);
            
        var filter = new ContactFilter2D();
        filter.SetLayerMask(LayerMask.GetMask("Unit"));
            
        var results = new List<Collider2D>();
        int hitCollidersAmount = Physics2D.OverlapCircle(transform.position, explosionRadius, filter, results);
        if (hitCollidersAmount <= 0)
            return;

        for (int i = 0; i < hitCollidersAmount; i++)
        {
            Collider2D hit = results[i];
            if (!hit.gameObject.TryGetComponent(out Character character)) 
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
