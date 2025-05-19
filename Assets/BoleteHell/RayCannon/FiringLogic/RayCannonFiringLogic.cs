using System;
using BoleteHell.Rays;
using Lasers;
using UnityEngine;

[Serializable]
public abstract class RayCannonFiringLogic
{
    //[SerializeField] protected float reloadTime;
    //[SerializeField] protected int numBulletsPerShot;
    //[SerializeField] protected float spreadAngle;
    //[SerializeField] protected float precision;
    protected float nextShootTime = 0f;  
    protected Vector2 _currentDirection;
    protected Vector3 _currentPos;

    public abstract void Shoot(Vector3 bulletSpawnPoint, Vector2 direction);
    public abstract void FinishFiring();
    public abstract void OnReset(LaserRenderer renderer);
}
