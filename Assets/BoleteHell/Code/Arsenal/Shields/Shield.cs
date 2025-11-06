using System;
using BoleteHell.Code.Arsenal.HitHandler;
using BoleteHell.Code.Arsenal.RayData;
using BoleteHell.Code.Arsenal.Rays;
using BoleteHell.Code.Arsenal.Shields.Advanced;
using BoleteHell.Code.Arsenal.Shields.Combos;
using BoleteHell.Code.Gameplay.Characters;
using BoleteHell.Code.Gameplay.Damage.Effects;
using BoleteHell.Code.Monetization;
using UnityEngine;
using Zenject;

namespace BoleteHell.Code.Arsenal.Shields
{
    /// <summary>
    /// ENHANCED SHIELD - Now with progression, combos, and monetization
    /// </summary>
    public class Shield : MonoBehaviour, ITargetable
    {
        [SerializeField] 
        private ShieldData shieldInfo;
        
        [SerializeField]
        private float currentShieldHealth;
        
        [SerializeField]
        private ParticleSystem hitParticles;
        
        [SerializeField]
        private ParticleSystem activeGlowEffect;
        
        private MeshRenderer meshRenderer;
        private Character _owner;
        private ShieldSkin _currentSkin;
        private bool isDestroyed = false;
        private int reflectionCount = 0;
        private float totalDamageAbsorbed = 0f;

        [Inject(Optional = true)]
        private ShieldComboTracker _comboTracker;

        [Inject]
        private IStatusEffectService _statusEffectService;

        private void Awake()
        {
            meshRenderer = GetComponent<MeshRenderer>();
        }

        private void Start()
        {
            if (shieldInfo != null)
            {
                Destroy(gameObject, shieldInfo.despawnTime);
                currentShieldHealth = shieldInfo.BaseHealth;
            }
        }

        public void SetLineInfo(ShieldData info, Character owner)
        {
            shieldInfo = info;
            _owner = owner;
            
            // Apply skin if available
            ApplySkin(info.DefaultSkin);
            
            currentShieldHealth = shieldInfo.BaseHealth;
        }
        
        // NEW: Cosmetic skin system
        public void ApplySkin(ShieldSkin skin)
        {
            if (skin == null) 
            {
                // Use default appearance
                Material mat = new Material(meshRenderer.material)
                {
                    color = shieldInfo.color
                };
                meshRenderer.material = mat;
                return;
            }
            
            _currentSkin = skin;
            
            // Apply premium material
            if (skin.ShieldMaterial != null)
            {
                meshRenderer.material = skin.ShieldMaterial;
            }
            
            // Spawn trail effect
            if (skin.TrailEffect != null)
            {
                Instantiate(skin.TrailEffect, transform.position, Quaternion.identity, transform);
            }
        }

        private Vector2 OnRayHitShield(Vector2 incomingDirection, RaycastHit2D hitPoint, LaserInstance laserInstance, LaserCombo laser, IFaction instigator)
        {
            if (shieldInfo.Equals(null))
            {
                Debug.LogError($"{name} has no lineInfo setup it should be set before calling this");
            }
            
            // NEW: Check if shield is destroyed
            if (isDestroyed) return incomingDirection;
            
            // NEW: Damage shield health
            float incomingDamage = CalculateLaserDamage(laser);
            currentShieldHealth -= incomingDamage;
            
            if (currentShieldHealth <= 0)
            {
                DestroyShield();
                return incomingDirection;
            }
            
            foreach (ShieldEffect effect in shieldInfo.shieldEffect)
            {
                if (instigator.IsAffected(effect.affectedSide, _owner))
                {
                    _statusEffectService.AddStatusEffect(laserInstance, effect.statusEffectConfig);
                }
            }
            
            laserInstance.MakeLaserNeutral();
            
            // NEW: Handle advanced shield types
            Vector2 resultDirection = ProcessAdvancedShieldLogic(incomingDirection, hitPoint, laser, laserInstance);
            
            // NEW: Track combo
            if (_comboTracker != null)
            {
                _comboTracker.RegisterShieldHit(shieldInfo.ShieldTypeId, true);
            }
            
            // NEW: Track stats
            reflectionCount++;
            totalDamageAbsorbed += incomingDamage;
            
            // NEW: Visual feedback
            SpawnHitEffect(hitPoint.point);
            
            return resultDirection;
        }
        
        // NEW: Advanced shield mechanics handler
        private Vector2 ProcessAdvancedShieldLogic(Vector2 incomingDirection, RaycastHit2D hitPoint, LaserCombo laser, LaserInstance laserInstance)
        {
            Vector2 baseResult = shieldInfo.onHitLogic.ExecuteRay(incomingDirection, hitPoint, laser.CombinedRefractiveIndex);
            
            // Absorber shield - convert damage to energy
            if (shieldInfo.onHitLogic is AbsorberLogic absorber)
            {
                float damage = CalculateLaserDamage(laser);
                float energyGain = absorber.CalculateEnergyGain(damage);
                if (_owner.Energy != null)
                {
                    _owner.Energy.Replenish(energyGain / _owner.Energy.regenRate);
                }
            }
            
            // Prismatic shield - split into multiple beams
            else if (shieldInfo.onHitLogic is PrismaticSplitLogic prismatic)
            {
                var directions = prismatic.GetSplitDirections(incomingDirection, hitPoint);
                // TODO: Spawn additional laser instances for each split
            }
            
            return baseResult;
        }
        
        private void DestroyShield()
        {
            isDestroyed = true;
            Destroy(gameObject, 0.5f);
        }
        
        private void SpawnHitEffect(Vector2 position)
        {
            if (hitParticles != null)
            {
                var particles = Instantiate(hitParticles, position, Quaternion.identity);
                Destroy(particles.gameObject, 2f);
            }
        }

        public void OnHit(ITargetable.Context ctx, Action<ITargetable.Response> callback = null)
        {
            LayerMask layerMask = ~LayerMask.GetMask("IgnoreProjectile");
            RaycastHit2D hit = Physics2D.Raycast(ctx.Position, ctx.Direction, Mathf.Infinity, layerMask);
            Debug.DrawRay(hit.point, -ctx.Direction * 5, Color.yellow, 1f);
            Debug.DrawRay(hit.point, hit.normal * 5, Color.green, 1f);

            if (!hit)
            {
                return;
            }
            
            if (ctx.Data is not LaserCombo laser)
            {
                Debug.LogWarning($"Hit data is not a CombinedLaser. Ignored hit.");
                return;
            }
            
            Vector3 newDirection = OnRayHitShield(ctx.Direction, hit, ctx.Projectile, laser, ctx.Instigator);
            Debug.DrawRay(hit.point, newDirection * 5, Color.red, 1f);
            callback?.Invoke(new ITargetable.Response(ctx) { Direction = newDirection });
        }
        
        // NEW: Stats accessors for achievements
        public int GetReflectionCount() => reflectionCount;
        public float GetTotalDamageAbsorbed() => totalDamageAbsorbed;
        
        // Helper method to calculate total damage from laser combo
        private float CalculateLaserDamage(LaserCombo laser)
        {
            float totalDamage = 0f;
            foreach (var laserData in laser.Data)
            {
                totalDamage += laserData.baseDamage;
            }
            return totalDamage;
        }
    }
}

