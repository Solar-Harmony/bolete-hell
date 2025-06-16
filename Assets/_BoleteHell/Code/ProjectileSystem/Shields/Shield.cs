using _BoleteHell.Code.ProjectileSystem.HitHandler;
using Data.Rays;
using UnityEngine;

namespace Shields
{
    public class Shield : MonoBehaviour, IHitHandler
    {
        [SerializeField] private ShieldData _lineInfo;

        public void SetLineInfo(ShieldData lineInfo)
        {
            _lineInfo = lineInfo;
        }

        public Vector3 OnRayHitLine(Vector3 incomingDirection, RaycastHit2D hitPoint, float lightRefractiveIndice)
        {
            if (_lineInfo.Equals(null))
                Debug.LogError($"{name} has no lineInfo setup it should be set before calling this");

            return _lineInfo.OnRayHit(incomingDirection, hitPoint, lightRefractiveIndice);
        }

        public void OnHit(IHitHandler.Context ctx)
        {
            LayerMask layerMask = ~LayerMask.GetMask("Projectile");
            RaycastHit2D hit = Physics2D.Raycast(gameObject.transform.position , ctx.Direction, Mathf.Infinity,layerMask);

            Debug.Log($"Raycast hit: {hit.collider?.name ?? "nothing"} at position {hit.point}");
      
            if (!hit) return;
         
            if (!ctx.Source.TryGetComponent(out LaserProjectileMovement projectile))
            {
                Debug.LogWarning($"Source of hit is not a LaserProjectileMovement. Ignored hit.");
                return;
            }
            
            if (ctx.Data is not CombinedLaser laser)
            {
                Debug.LogWarning($"Hit data is not a CombinedLaser. Ignored hit.");
                return;
            }
            
            Vector3 newDirection = OnRayHitLine(ctx.Direction, hit, laser.CombinedRefractiveIndex);
            projectile.SetDirection(newDirection);
        }
    }
}