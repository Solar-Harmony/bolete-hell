using System;
using BoleteHell.Code.Audio.BoleteHell.Models;
using UnityEngine;

namespace BoleteHell.Gameplay.Lasers
{
    [Serializable]
    public class SimpleDamageOnHit : RayHitLogic
    {
        public override void OnHitImpl(Vector2 hitPosition, IDamageable damageable)
        {
           
        }
    }
}