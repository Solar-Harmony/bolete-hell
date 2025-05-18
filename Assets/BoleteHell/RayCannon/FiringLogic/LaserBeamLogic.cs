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
    
    
    private readonly List<Vector3> _rayPositions = new();
    
    private LaserRenderer reservedRenderer;
    public override void StartFiring()
    {
        base.StartFiring();
        UpdateChargeTime();
    }
    
    
    protected override void InitLaserData()
    {
        _modifiableLaserData = ScriptableObjectCloner.CloneScriptableObject(laserData);
    }

    public override void Shoot(Vector3 bulletSpawnPoint, Vector2 direction)
    {
        if (!(Time.time >= nextShootTime)) return;
        //Devrait le reserver dans le start firing et juste utilisé le même tant qu'il tire mais faudrais fix des vchose et ça me tente pas 
        reservedRenderer = LineRendererPool.Instance.Get();

        Cast(bulletSpawnPoint, direction);
        UpdateChargeTime();
    }

    public override void FinishFiring()
    {
        LineRendererPool.Instance.Release(reservedRenderer);
    }



    public void Cast(Vector3 bulletSpawnPoint, Vector2 direction)
    {
        _currentPos = bulletSpawnPoint;
        _rayPositions.Add(_currentPos);
        _currentDirection = direction;

        for (int i = 0; i <= laserData.maxNumberOfBounces; i++)
        {
            RaycastHit2D hit = Physics2D.Raycast(_currentPos, _currentDirection,laserData.maxRayDistance);
            if (!hit)
            {
                Debug.DrawRay(_currentPos, _currentDirection * laserData.maxRayDistance, Color.black);
                _rayPositions.Add((Vector2)_currentPos + _currentDirection * laserData.maxRayDistance);
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
        
        reservedRenderer.DrawRay(_rayPositions,laserData.Color,lifeTime);
        _rayPositions.Clear();
    }

    public void OnHitEnemy(Vector2 hitPosition)
    {
        _rayPositions.Add(hitPosition);
        laserData.logic.OnHit();
    }

    public void OnHitShield(RaycastHit2D hitPoint, Line lineHit)
    {
        Debug.DrawLine(_currentPos, hitPoint.point, Color.blue);
        _currentDirection = lineHit.OnRayHitLine(_currentDirection, hitPoint, laserData.LightRefractiveIndex);
        _currentPos = hitPoint.point + _currentDirection * 0.01f;
        _rayPositions.Add(_currentPos);
    }

    private void UpdateChargeTime()
    {
        nextShootTime = Time.time + chargingTime;
    }

}
