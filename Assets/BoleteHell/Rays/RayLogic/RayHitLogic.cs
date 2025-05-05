using System;
using UnityEngine;

[System.Serializable]
public abstract class RayHitLogic
{
    [SerializeField] public float baseDamage = 10f;
    [SerializeField ]public float projectileSpeed = 20f;
    public abstract void OnHit();
    
}
