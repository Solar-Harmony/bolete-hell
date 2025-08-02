using System;
using BoleteHell.Code.Arsenal.HitHandler;
using BoleteHell.Code.Gameplay.Damage;
using UnityEngine;

namespace BoleteHell.Code.Gameplay.Base
{
    public class Base : MonoBehaviour, ITargetable, IDamageable
    {
        public void OnHit(ITargetable.Context ctx, Action<ITargetable.Response> callback = null)
        {
            throw new NotImplementedException();
        }

        public Health Health { get; } = new();
    }
}