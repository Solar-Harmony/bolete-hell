using System;
using System.Collections.Generic;
using BoleteHell.Rays;
using BoleteHell.Utils;
using Lasers;
using Shields;
using UnityEngine;


[Serializable]
public class LaserProjectileLogic : RayCannonFiringLogic
{
    [SerializeField] private float timeBetweenShots = 0.2f;
    [SerializeField] private float projectileSpeed = 10f;
    [SerializeField] private float projectileLifeTime = 1.5f;
    [SerializeField] private LaserProjectileData laserData;
    private LaserProjectileData _modifiableLaserDate;

    protected override void InitLaserData()
    {
        _modifiableLaserDate = ObjectInstantiator.CloneScriptableObject(laserData);
    }
    
    public override void StartFiring()
    {
        
    }
    
    //Bug: si on tire collé sur un shield le projectile passe a travers, devrais peut-être empecher de tirer si le projectile spawn dans un mur
    public override void Shoot(Vector3 bulletSpawnPoint, Vector2 direction)
    {
        if (!(Time.time >= nextShootTime)) return;
        
        nextShootTime = Time.time + timeBetweenShots;

        //Créé seulement un point de début et un point de fin
        List<Vector3> positions = new List<Vector3> { Vector3.zero, Vector3.up * _modifiableLaserDate.laserLenght };
        LaserRenderer reservedRenderer = LineRendererPool.Instance.Get();
        reservedRenderer.transform.position = bulletSpawnPoint;
        reservedRenderer.DrawRay(positions, _modifiableLaserDate.Color, projectileLifeTime,this);
        reservedRenderer.SetupProjectileLaser(_modifiableLaserDate, direction, projectileSpeed);
    }
    
    public override void OnReset(LaserRenderer renderer)
    {
        LineRendererPool.Instance.Release(renderer);
    }
    
    //La logique du onHit du projectile se trouve dans le LaserProjectileMovement car le collider est la.
    
    public override void FinishFiring()
    {
        
    }
}
