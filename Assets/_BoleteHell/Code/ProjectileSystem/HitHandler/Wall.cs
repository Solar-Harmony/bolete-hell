using System;
using UnityEngine;

namespace _BoleteHell.Code.ProjectileSystem.HitHandler
{
    public class Wall : MonoBehaviour, IHitHandler
    {
        public void OnHit(IHitHandler.Context ctx, Action<IHitHandler.Response> callback = null)
        {
            callback?.Invoke(new IHitHandler.Response(ctx) { RequestDestroy = true });
        }
    }
}