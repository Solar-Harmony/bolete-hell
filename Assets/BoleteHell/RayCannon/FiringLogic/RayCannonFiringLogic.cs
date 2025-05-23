using System;
using UnityEngine;
using Ray = Lasers.Ray;

[Serializable]
public abstract class RayCannonFiringLogic
{
    [SerializeField] protected float reloadTime;
    [SerializeField] protected int numBulletsPerShot;
    [SerializeField] protected float spreadAngle;
    [SerializeField] protected float precision;
    protected float nextShootTime = 0f;  
    protected Vector2 _currentDirection;
    protected Vector3 _currentPos;
    protected Ray currentRay;

    // FIXME: Get rid of this init method it's confusing if you forget to call it everything breaks lol
    public virtual void StartFiring(Ray ray)
    {
        currentRay = ray;
    }
    public abstract void Shoot(Vector3 bulletSpawnPoint, Vector2 direction);
    public abstract void FinishFiring();

    //Nécéssaire vu qu'on dirait que les données ne se font pas reset quand on quitte le play mode for some reason kms
    
    public void ResetData()
    {
        nextShootTime = 0f;
    }

}
