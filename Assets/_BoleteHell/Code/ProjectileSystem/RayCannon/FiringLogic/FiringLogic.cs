using System;
using System.Collections.Generic;
using BoleteHell.Rays;
using Data.Cannons;
using Data.Rays;
using Lasers;
using UnityEngine;

public abstract class FiringLogic
{
    protected Vector2 CurrentDirection;
    protected Vector3 CurrentPos;
    
    public abstract void Shoot(Vector3 bulletSpawnPoint, Vector2 direction, RayCannonData data, CombinedLaser laser, GameObject instigator = null);
    public abstract void FinishFiring();
}
