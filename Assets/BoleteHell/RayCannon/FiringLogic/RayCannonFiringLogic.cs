using System;
using System.Collections.Generic;
using BoleteHell.Rays;
using Data.Cannons;
using Data.Rays;
using Lasers;
using UnityEngine;

public abstract class RayCannonFiringLogic
{
    protected float NextShootTime = 0f;  
    protected Vector2 CurrentDirection;
    protected Vector3 CurrentPos;

    protected RayCannonFiringLogic()
    {
        NextShootTime = 0f;
    }
    
    public abstract void Shoot(Vector3 bulletSpawnPoint, Vector2 direction,RayCannonData data,CombinedLaser laser);
    public abstract void FinishFiring();
    public abstract void OnReset(LaserRenderer renderer);
}
