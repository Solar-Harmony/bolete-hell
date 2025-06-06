using System;
using System.Collections.Generic;
using BoleteHell.Rays;
using BoleteHell.Utils;
using Data.Cannons;
using Data.Rays;
using Lasers;
using Shields;
using UnityEngine;


public class LaserProjectileLogic : FiringLogic
{
    
    //Bug: si on tire collé sur un shield le projectile passe a travers, devrais peut-être empecher de tirer si le projectile spawn dans un mur

    public override void Shoot(Vector3 bulletSpawnPoint, Vector2 direction,RayCannonData data,CombinedLaser laser)
    {
        //Créé seulement un point de début et un point de fin
        LaserRenderer reservedRenderer = LaserRendererPool.Instance.Get();
        List<Vector3> positions = new List<Vector3> { Vector3.zero, Vector3.up * reservedRenderer.LaserLength };
        reservedRenderer.transform.position = bulletSpawnPoint;
        reservedRenderer.DrawRay(positions, laser.CombinedColor, data.LifeTime,this);
        reservedRenderer.SetupProjectileLaser(laser.CombinedRefractiveIndex, direction,laser);
    }
    
    //La logique du onHit du projectile se trouve dans le LaserProjectileMovement car le collider est la.
    public override void FinishFiring()
    {
        
    }
    
}
