using System;
using BoleteHell.Code.Arsenal.RayData;
using BoleteHell.Code.Gameplay.Damage;
using UnityEngine;

namespace BoleteHell.Code.Arsenal.Rays.RayLogic
{
    [Serializable]
    public class SimpleDamageOnHit : RayHitLogic
    {
        public override void OnHitImpl(Vector2 hitPosition, IDamageable damageable)
        {
           
        }
    }
}