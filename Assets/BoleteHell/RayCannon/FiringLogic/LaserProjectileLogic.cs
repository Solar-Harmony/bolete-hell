using System;
using System.Collections.Generic;
using BoleteHell.Rays;
using Lasers;
using Shields;
using UnityEngine;


[Serializable]
public class LaserProjectileLogic : RayCannonFiringLogic
{
    [SerializeField] private float timeBetweenShots = 0.2f;
    [SerializeField] private float projectileSpeed = 10f;
    [SerializeField] private float projectileLifeTime = 1.5f;
    //Bug: si on tire collé sur un shield le projectile passe a travers, devrais peut-être empecher de tirer si le projectile spawn dans un mur
    public override void Shoot(Vector3 bulletSpawnPoint, Vector2 direction)
    {
        if (!(Time.time >= nextShootTime)) return;
        
        nextShootTime = Time.time + timeBetweenShots;

        if (!currentRay)
        {
            Debug.LogError("You forgot to call StartFiring on the RayCannonFiringLogic. Also, this is stupid. We shouldn't have to call it manually.");
            return;
        }

        //Créé seulement un point de début et un point de fin
        List<Vector3> positions = new List<Vector3> { Vector3.zero, Vector3.up * currentRay.raylength };
        //Le line renderer va être released par le projectile lui même
        LaserRenderer laser = LineRendererPool.Instance.Get();
        laser.transform.position = bulletSpawnPoint;
        laser.DrawRay(positions, currentRay.Color, projectileLifeTime);
        laser.SetupProjectileLaser(currentRay, direction, projectileSpeed,this);
    }
    
    //La logique du onHit du projectile se trouve dans le LaserProjectileMovement car le collider est la.
    
    public override void FinishFiring()
    {
        
    }
}
