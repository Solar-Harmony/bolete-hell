using System;
using UnityEngine;

namespace BoleteHell.Arsenals.Shields.ShieldsLogic
{
    [Serializable]
    public class MirrorLogic : IShieldHitLogic
    {
        public Vector2 ExecuteRay(Vector3 incomingDirection, RaycastHit2D hitPoint, float lightRefractiveIndice)
        {
            // if (instigator == _entityFinder.GetPlayer().gameObject)
            // {
            //     _statusEffectService.AddStatusEffect(laserInstance, shieldInfo.statusEffectConfig);
            // }
            
            return Vector2.Reflect(incomingDirection, hitPoint.normal);
        }
    }
}