using System;
using System.Collections.Generic;
using UnityEngine;
using Ray = Lasers.Ray;

[Serializable]
public abstract class RayCannonFiringLogic
{
    [SerializeField] protected float reloadTime;
    [SerializeField] protected int numBulletsPerShot;
    [SerializeField] protected float spreadAngle;
    [SerializeField] protected float precision;
    protected Vector2 _currentDirection;
    protected Vector3 _currentPos;
    protected Ray currentRay;

    public virtual void StartFiring(Ray ray)
    {
        currentRay = ray;
    }

    public abstract void Shoot(Vector3 bulletSpawnPoint, Vector2 direction);
    public abstract void FinishFiring();
}
