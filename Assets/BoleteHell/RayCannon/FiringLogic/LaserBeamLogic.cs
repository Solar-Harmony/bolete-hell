using System;
using System.Collections.Generic;
using BoleteHell.Rays;
using Lasers;
using Shields;
using UnityEngine;

[Serializable]
public class LaserBeamLogic:RayCannonFiringLogic
{
    [SerializeField] private float chargingTime = 0.5f;
    [SerializeField] private float lifeTime = 0.1f;
    [SerializeField] private LaserBeamData laserData;
    private LaserBeamData _modifiableLaserData;
    private LaserRenderer reservedRenderer;

    
    private readonly List<Vector3> _rayPositions = new();
    
    public override void StartFiring()
    {
        reservedRenderer = LineRendererPool.Instance.Get();
        UpdateChargeTime();
    }


    public override void OnReset(LaserRenderer renderer)
    {
        
    }

    protected override void InitLaserData()
    {
        _modifiableLaserData = ScriptableObjectCloner.CloneScriptableObject(laserData);
    }

    public override void Shoot(Vector3 bulletSpawnPoint, Vector2 direction)
    {
        if (!(Time.time >= nextShootTime)) return;
        Cast(bulletSpawnPoint, direction);
        UpdateChargeTime();
    }

    public override void FinishFiring()
    {
        LineRendererPool.Instance.Release(reservedRenderer);
    }
    
    


    private void Cast(Vector3 bulletSpawnPoint, Vector2 direction)
    {
        _currentPos = bulletSpawnPoint;
        _rayPositions.Add(_currentPos);
        _currentDirection = direction;

        for (int i = 0; i <= _modifiableLaserData.maxNumberOfBounces; i++)
        {
            RaycastHit2D hit = Physics2D.Raycast(_currentPos, _currentDirection,_modifiableLaserData.maxRayDistance);
            if (!hit)
            {
                Debug.DrawRay(_currentPos, _currentDirection * _modifiableLaserData.maxRayDistance, Color.black);
                _rayPositions.Add((Vector2)_currentPos + _currentDirection * _modifiableLaserData.maxRayDistance);
                break;
            }

            if (hit.transform.gameObject.TryGetComponent(out Line lineHit))
            {
                OnHitShield(hit, lineHit);
            }
            //Devrait check le health component de la personne pour que ça fonctionne si on touche un ennemi ou le joueur
            else if (hit.collider.CompareTag("Enemy"))
            {
                OnHitEnemy(hit.point);
            }
        }
        
        reservedRenderer.DrawRay(_rayPositions,_modifiableLaserData.Color,lifeTime,this);
        _rayPositions.Clear();
    }

    private void OnHitEnemy(Vector2 hitPosition)
    {
        _rayPositions.Add(hitPosition);
        _modifiableLaserData.logic.OnHit();
    }

    private void OnHitShield(RaycastHit2D hitPoint, Line lineHit)
    {
        Debug.DrawLine(_currentPos, hitPoint.point, Color.blue);
        _currentDirection = lineHit.OnRayHitLine(_currentDirection, hitPoint, _modifiableLaserData.LightRefractiveIndex);
        _currentPos = hitPoint.point + _currentDirection * 0.01f;
        _rayPositions.Add(_currentPos);
    }

    private void UpdateChargeTime()
    {
        nextShootTime = Time.time + chargingTime;
    }

}
