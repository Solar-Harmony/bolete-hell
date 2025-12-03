using System;
using BoleteHell.Code.Arsenal.HitHandler;
using BoleteHell.Code.Arsenal.RayData;
using BoleteHell.Code.Arsenal.Rays;
using BoleteHell.Code.Gameplay.Damage.Effects;
using BoleteHell.Gameplay.Characters;
using BoleteHell.Gameplay.StatusEffectImpl;
using UnityEngine;
using Zenject;

namespace BoleteHell.Code.Arsenal.Shields
{
    [RequireComponent(typeof(MeshRenderer), typeof(HealthComponent))]
    public class Shield : MonoBehaviour, ITargetable
    {
        [SerializeField] 
        private ShieldData shieldInfo;
        
        private MeshRenderer meshRenderer;
        
        private Coroutine despawnCoroutine;

        private GameObject _owner;

        private HealthComponent health;

        [Inject]
        private IStatusEffectService _statusEffectService;

        private void Awake()
        {
            meshRenderer = GetComponent<MeshRenderer>();
            //Présentement le nobmre de max hp est obliger d'être pareil pour chaque shield car il esst défini dans le prefab
            //Chaque shield a le même prefab et est modifier selon son shieldData
            
            //Il faudrait définir le max hp dans les shieldData et override le maxHp de HealthComponent
            //mais ça peut rendre la modification du max hp des shields moins intuitif car modifier le healthComponent ne va pas changer la vie réellement 
            health = GetComponent<HealthComponent>();
            health.OnDeath += DestroyShield;
        }

        private void Start()
        {
            Destroy(gameObject, shieldInfo.despawnTime);
        }

        private void DestroyShield()
        {
            Destroy(gameObject);
        }

        public void SetLineInfo(ShieldData info, GameObject owner)
        {
            shieldInfo = info;
            Material mat = new Material(meshRenderer.material)
            {
                color = shieldInfo.color
            };
            meshRenderer.material = mat;
            _owner = owner;
        }

        private Vector2 OnRayHitShield(Vector2 incomingDirection, RaycastHit2D hitPoint, LaserInstance laserInstance, LaserCombo laser, GameObject instigator)
        {
            if (shieldInfo.Equals(null))
                Debug.LogError($"{name} has no lineInfo setup it should be set before calling this");
            
            foreach (ShieldEffect effect in shieldInfo.shieldEffect)
            {
                bool hasInstigatorFaction = instigator.TryGetComponent<FactionComponent>(out var instigatorFaction);
                bool hasOwnerFaction = _owner.TryGetComponent<FactionComponent>(out var ownerFaction);

                if (hasInstigatorFaction && hasOwnerFaction && instigatorFaction.IsAffected(effect.AffectedSide, ownerFaction))
                {
                    _statusEffectService.AddStatusEffect(laserInstance.gameObject, effect.statusEffectConfig );
                }
            }
            
            health.TakeDamage(1);
            
            laserInstance.MakeLaserNeutral();
            
            return shieldInfo.onHitLogic.ExecuteRay(incomingDirection, hitPoint, laser.CombinedRefractiveIndex);
        }

        public void OnHit(ITargetable.Context ctx, Action<ITargetable.Response> callback = null)
        {
            if (ctx.Data is not LaserCombo laser)
            {
                Debug.LogWarning($"Hit data is not a CombinedLaser. Ignored hit.");
                return;
            }
            
            Vector3 newDirection = OnRayHitShield(ctx.Direction, ctx.RayHit, ctx.Projectile, laser, ctx.Instigator);
            
            callback?.Invoke(ctx.Projectile.isProjectile || !shieldInfo.onHitLogic.ShouldBlocklaser
                ? new ITargetable.Response(ctx) { Direction = newDirection }
                : new ITargetable.Response(ctx) { Direction = newDirection, BlockProjectile = true });
        }

        private void OnDestroy()
        {
            health.OnDeath -= DestroyShield;
        }
    }
}