using System;
using BoleteHell.Gameplay.Characters;
using UnityEngine;

namespace BoleteHell.Code.Arsenal.Rays.RayLogic
{
    [Serializable]
    public class SimpleDamageOnHit : RayHitLogic
    {
        public override void OnHitImpl(Vector2 hitPosition, HealthComponent victim, LaserInstance laser)
        {
           
        }
    }
}