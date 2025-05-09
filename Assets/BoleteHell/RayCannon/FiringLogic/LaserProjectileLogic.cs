using System;
using System.Collections;
using System.Collections.Generic;
using Lasers;
using UnityEngine;
using Ray = Lasers.Ray;

[Serializable]
public class LaserProjectileLogic : RayCannonFiringLogic
{
    [SerializeField] private float timeBetweenShots = 0.2f;
    [SerializeField] private float projectileSPeed = 10f;
    private float currentTimer = 0f;

    public override void Shoot(Vector3 bulletSpawnPoint, Vector2 direction)
    {
        if (currentTimer <= 0)
        {
            currentTimer = timeBetweenShots;
            
            //Créé seulement un point de début et un point de fin
            List<Vector3> positions = new List<Vector3> { Vector3.zero, Vector3.up*currentRay.raylength };
            //Le line renderer va être released par le projectile lui même
            LaserRenderer laser = LineRendererPool.GetRandomAvailable();
            laser.transform.position = bulletSpawnPoint;
            laser.DrawRay(positions, currentRay.Color);
            laser.SetupProjectileLaser(currentRay.rayWidth,currentRay.raylength,direction,projectileSPeed);
        }
        else
        {
            currentTimer -= Time.deltaTime;
        }
    }

    public override void FinishFiring()
    {
        currentTimer = timeBetweenShots;
    }
    

    //TODO: pas trop le choix d'Avoir le déclanchage? du on hit dans l'objet qu'on instantie car il a le collider
}
