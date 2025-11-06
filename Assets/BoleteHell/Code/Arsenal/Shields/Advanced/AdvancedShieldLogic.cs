using System;
using System.Collections.Generic;
using BoleteHell.Code.Arsenal.Shields.ShieldsLogic;
using UnityEngine;

namespace BoleteHell.Code.Arsenal.Shields.Advanced
{
    /// <summary>
    /// ADVANCED SHIELD MECHANICS - High skill ceiling
    /// Absorber shield converts incoming damage to energy
    /// Rewards skilled play and positioning
    /// </summary>
    [Serializable]
    public class AbsorberLogic : IShieldHitLogic
    {
        [SerializeField]
        private float absorptionEfficiency = 0.75f; // Convert 75% of damage to energy
        
        [SerializeField]
        private float maxAbsorptionPerHit = 50f;
        
        [SerializeField]
        private bool reflectsOverflow = true; // Excess damage reflects back
        
        public Vector2 ExecuteRay(Vector3 incomingDirection, RaycastHit2D hitPoint, float lightRefractiveIndice)
        {
            // Absorbed - no reflection
            // Energy conversion happens in Shield.cs
            return Vector2.zero;
        }
        
        public float CalculateEnergyGain(float incomingDamage)
        {
            return Mathf.Min(incomingDamage * absorptionEfficiency, maxAbsorptionPerHit);
        }
    }
    
    /// <summary>
    /// Prismatic shield - splits laser into multiple beams
    /// Creates tactical depth - can clear multiple enemies or self-damage
    /// PREMIUM FEATURE - high skill, high reward
    /// </summary>
    [Serializable]
    public class PrismaticSplitLogic : IShieldHitLogic
    {
        [SerializeField]
        private int splitCount = 3;
        
        [SerializeField]
        private float spreadAngle = 45f;
        
        [SerializeField]
        private float damagePerBeam = 0.5f; // Each beam does 50% of original
        
        public Vector2 ExecuteRay(Vector3 incomingDirection, RaycastHit2D hitPoint, float lightRefractiveIndice)
        {
            // Returns primary beam direction
            // Additional beams spawned separately
            return Vector2.Reflect(incomingDirection, hitPoint.normal);
        }
        
        public List<Vector2> GetSplitDirections(Vector3 incomingDirection, RaycastHit2D hitPoint)
        {
            List<Vector2> directions = new List<Vector2>();
            Vector2 baseReflection = Vector2.Reflect(incomingDirection, hitPoint.normal);
            
            float angleStep = spreadAngle / (splitCount - 1);
            float startAngle = -spreadAngle / 2f;
            
            for (int i = 0; i < splitCount; i++)
            {
                float angle = startAngle + (angleStep * i);
                Vector2 rotated = Quaternion.Euler(0, 0, angle) * baseReflection;
                directions.Add(rotated);
            }
            
            return directions;
        }
    }
    
    /// <summary>
    /// Temporal shield - slows projectiles in radius
    /// Creates "bullet time" moments - feels amazing
    /// LEGENDARY tier unlock
    /// </summary>
    [Serializable]
    public class TemporalSlowLogic : IShieldHitLogic
    {
        [SerializeField]
        private float slowRadius = 5f;
        
        [SerializeField]
        private float slowFactor = 0.3f; // 70% slower
        
        [SerializeField]
        private float slowDuration = 2f;
        
        [SerializeField]
        private ParticleSystem timeDistortionVFX;
        
        public Vector2 ExecuteRay(Vector3 incomingDirection, RaycastHit2D hitPoint, float lightRefractiveIndice)
        {
            // Projectile continues but slowed
            return incomingDirection;
        }
    }
    
    /// <summary>
    /// Quantum shield - teleports projectiles to random location
    /// High chaos, high fun factor
    /// Can teleport enemy projectiles back at them
    /// MYTHIC tier - whale bait
    /// </summary>
    [Serializable]
    public class QuantumTeleportLogic : IShieldHitLogic
    {
        [SerializeField]
        private float teleportRadius = 10f;
        
        [SerializeField]
        private bool preferEnemyDirection = true;
        
        [SerializeField]
        private ParticleSystem teleportVFX;
        
        [SerializeField]
        private AudioClip teleportSound;
        
        public Vector2 ExecuteRay(Vector3 incomingDirection, RaycastHit2D hitPoint, float lightRefractiveIndice)
        {
            // Calculate teleport destination
            if (preferEnemyDirection)
            {
                // Teleport projectile towards nearest enemy
                // Implementation in Shield.cs
            }
            
            return incomingDirection; // Direction maintained after teleport
        }
    }
    
    /// <summary>
    /// Adaptive shield - changes properties based on incoming damage type
    /// Deep tactical system - rewards game knowledge
    /// </summary>
    [Serializable]
    public class AdaptiveShieldLogic : IShieldHitLogic
    {
        [SerializeField]
        private float adaptationSpeed = 0.5f;
        
        private Dictionary<string, float> damageTypeResistances = new Dictionary<string, float>();
        
        [SerializeField]
        private float maxResistance = 0.8f; // 80% resistance cap
        
        public Vector2 ExecuteRay(Vector3 incomingDirection, RaycastHit2D hitPoint, float lightRefractiveIndice)
        {
            // Learns and adapts to repeated damage types
            return Vector2.Reflect(incomingDirection, hitPoint.normal);
        }
        
        public void AdaptToType(string damageType)
        {
            if (!damageTypeResistances.ContainsKey(damageType))
            {
                damageTypeResistances[damageType] = 0f;
            }
            
            damageTypeResistances[damageType] = Mathf.Min(
                damageTypeResistances[damageType] + adaptationSpeed,
                maxResistance
            );
        }
    }
    
    /// <summary>
    /// Reactive shield - auto counter-attacks
    /// Lower skill floor - good for casual players (monetization target)
    /// </summary>
    [Serializable]
    public class ReactiveCounterLogic : IShieldHitLogic
    {
        [SerializeField]
        private float counterDamageMultiplier = 1.5f;
        
        [SerializeField]
        private bool autoTargetsSource = true;
        
        [SerializeField]
        private GameObject counterProjectilePrefab;
        
        public Vector2 ExecuteRay(Vector3 incomingDirection, RaycastHit2D hitPoint, float lightRefractiveIndice)
        {
            // Spawns counter-projectile back at source
            return -incomingDirection; // Simple reflection for display
        }
    }
}

