using UnityEngine;

namespace Lasers.RayLogic
{
    public class PoisonOnHit : RayHitLogic
    {
        public override void OnHit()
        {
            Debug.Log("Get well soon");
        }
    }
}