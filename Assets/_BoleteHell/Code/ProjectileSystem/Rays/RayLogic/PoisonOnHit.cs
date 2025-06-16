using System.Collections;
using _BoleteHell.Code.Character;
using _BoleteHell.Code.ProjectileSystem.Rays.RayLogic;
using UnityEngine;

namespace Lasers.RayLogic
{
    public class PoisonOnHit : RayHitLogic
    {
        //will need a position and the hit characters stats script so i can modify it
        public override void OnHit(Vector2 hitPosition, Health hitCharacterHealth)
        {
            //Physics2D.OverlapCircle()
            hitCharacterHealth.TakeDamage(baseHitDamage);
            // TODO: Add poison effectto the character 
        }
    }
}