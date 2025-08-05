using System;
using BoleteHell.Code.Arsenal.HitHandler;
using BoleteHell.Code.Arsenal.RayData;
using UnityEngine;

namespace BoleteHell.Code.Arsenal.Shields
{
    public class Shield : MonoBehaviour, ITargetable
    {
        [SerializeField] private ShieldData lineInfo;

        public void SetLineInfo(ShieldData info)
        {
            lineInfo = info;
        }

        private Vector2 OnRayHitLine(Vector2 incomingDirection, RaycastHit2D hitPoint, float lightRefractiveIndice)
        {
            if (lineInfo.Equals(null))
                Debug.LogError($"{name} has no lineInfo setup it should be set before calling this");

            return lineInfo.OnRayHit(incomingDirection, hitPoint, lightRefractiveIndice);
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
            
            Vector3 newDirection = OnRayHitLine(ctx.Direction, hit, laser.CombinedRefractiveIndex);
            Debug.DrawRay(hit.point, newDirection * 5, Color.red, 1f);
            callback?.Invoke(new ITargetable.Response(ctx) { Direction = newDirection });
        }
    }
}