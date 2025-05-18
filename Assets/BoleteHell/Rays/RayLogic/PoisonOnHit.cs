using UnityEngine;

namespace Lasers.RayLogic
{
    public class PoisonOnHit : RayHitLogic
    {
        //will need a position and the hit characters stats script so i can modify it
        public override void OnHit(Vector2 hitPosition,Health hitCharacterHealth)
        {
            //Physics2D.OverlapCircle()
            Debug.Log("Get well soon");
        }
    }
}