using System;
using UnityEngine;

namespace BoleteHell.Code.Arsenal.HitHandler
{
    public class Wall : MonoBehaviour, ITargetable
    {
        public void OnHit(ITargetable.Context ctx, Action<ITargetable.Response> callback = null)
        {
            callback?.Invoke(new ITargetable.Response(ctx) { RequestDestroyProjectile = true });
        }
    }
}