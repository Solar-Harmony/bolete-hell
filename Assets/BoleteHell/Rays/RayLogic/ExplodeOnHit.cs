using System;
using UnityEngine;

[System.Serializable]
public class ExplodeOnHit:RayHitLogic
{
    [SerializeField] private float width;
    public override void OnHit()
    {
        Debug.Log("test");
    }
}
