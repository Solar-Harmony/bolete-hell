using UnityEngine;

public class PoisonOnHit:RayHitLogic
{
    public override void OnHit()
    {
        Debug.Log("Get well soon");
    }
}
