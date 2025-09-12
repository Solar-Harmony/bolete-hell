using System;
using BoleteHell.Code.Arsenal.HitHandler;
using BoleteHell.Code.Arsenal.RayData;
using UnityEngine;
using UnityEngine.Serialization;

namespace BoleteHell.Code.Arsenal.Shields
{
    public class Shield : MonoBehaviour, ITargetable
    {
        [FormerlySerializedAs("lineInfo")]
        [SerializeField] 
        private ShieldData shieldInfo;
        
        private MeshRenderer meshRenderer;
        
        private Coroutine despawnCoroutine;

        private void Awake()
        {
            meshRenderer = GetComponent<MeshRenderer>();
        }

        private void Start()
        {
            ((IRequestManualInject)shieldInfo).InjectDependencies();

            Destroy(gameObject, shieldInfo.despawnTime);
        }

        public void SetLineInfo(ShieldData info)
        {
            shieldInfo = info;
            Material mat = new Material(meshRenderer.material)
            {
                color = shieldInfo.color
            };
            meshRenderer.material = mat;
        }

        private Vector2 OnRayHitShield(Vector2 incomingDirection, RaycastHit2D hitPoint, LaserCombo laser, GameObject instigator)
        {
            if (shieldInfo.Equals(null))
                Debug.LogError($"{name} has no lineInfo setup it should be set before calling this");

            return shieldInfo.OnRayHit(incomingDirection, hitPoint, laser, instigator);
        }

        public void OnHit(ITargetable.Context ctx, Action<ITargetable.Response> callback = null)
        {
            LayerMask layerMask = ~LayerMask.GetMask("IgnoreProjectile");
            RaycastHit2D hit = Physics2D.Raycast(ctx.Position, ctx.Direction, Mathf.Infinity, layerMask);            // debug draw the hit
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
            
            Vector3 newDirection = OnRayHitShield(ctx.Direction, hit, laser, ctx.Instigator);
            Debug.DrawRay(hit.point, newDirection * 5, Color.red, 1f);
            callback?.Invoke(new ITargetable.Response(ctx) { Direction = newDirection });
        }
    }
}