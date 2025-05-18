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

    public abstract void StartFiring();
    public abstract void Shoot(Vector3 bulletSpawnPoint, Vector2 direction);
    public abstract void FinishFiring();
    public abstract void OnReset(LaserRenderer renderer);
    protected abstract void InitLaserData();

    public void Init()
    {
        InitLaserData();
        #if UNITY_EDITOR
        ResetData();
        #endif
    }
    
    //Nécéssaire vu qu'on dirait que les données ne se font pas reset quand on quitte le play mode for some reason kms
    public void ResetData()
    {
        nextShootTime = 0f;
    }

}
