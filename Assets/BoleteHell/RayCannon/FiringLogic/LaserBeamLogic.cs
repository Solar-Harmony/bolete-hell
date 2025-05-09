using System;
using System.Collections.Generic;
using Lasers;
using Shields;
using UnityEngine;
using Ray = Lasers.Ray;

[Serializable]
public class LaserBeamLogic:RayCannonFiringLogic
{
    [SerializeField] private float chargingSpeed;
    
    private readonly List<Vector3> _rayPositions = new();
    private LaserRenderer reservedRenderer;
    public override void StartFiring(Ray ray)
    {
        base.StartFiring(ray);
        reservedRenderer = LineRendererPool.GetRandomAvailable();
    }

    public override void Shoot(Vector3 bulletSpawnPoint, Vector2 direction)
    {
        Cast(bulletSpawnPoint,direction);
    }

    public override void FinishFiring()
    {
        LineRendererPool.Release(reservedRenderer.Id);
    }
    
    
    public void Cast(Vector3 bulletSpawnPoint, Vector2 direction)
    {
        _currentPos = bulletSpawnPoint;
        _rayPositions.Add(_currentPos);
        _currentDirection = direction;

        for (int i = 0; i <= currentRay.maxNumberOfBounces; i++)
        {
            RaycastHit2D hit = Physics2D.Raycast(_currentPos, _currentDirection,currentRay.maxRayDistance);
            if (!hit)
            {
                Debug.DrawRay(_currentPos, _currentDirection * currentRay.maxRayDistance, Color.black);
                _rayPositions.Add((Vector2)_currentPos + _currentDirection * currentRay.maxRayDistance);
                break;
            }

            if (hit.transform.gameObject.TryGetComponent(out Line lineHit))
            {
                OnHitWall(hit, lineHit);
            }
            //Devrait check le health component de la personne pour que ça fonctionne si on touche un ennemi ou le joueur
            else if (hit.collider.CompareTag("Enemy"))
            {
                OnHitEnemy(hit);
            }
        }
        
        reservedRenderer.DrawRay(_rayPositions,currentRay.Color);
        _rayPositions.Clear();
    }

    private void OnHitEnemy(RaycastHit2D hit)
    {
        _rayPositions.Add(hit.point);
        currentRay.logic.OnHit();
    }

    private void OnHitWall(RaycastHit2D hit, Line lineHit)
    {
        Debug.DrawLine(_currentPos, hit.point, Color.blue);
        _currentDirection = lineHit.OnRayHitLine(_currentDirection, hit, currentRay.LightRefractiveIndex);
        _currentPos = hit.point + _currentDirection * 0.01f;
        _rayPositions.Add(_currentPos);
    }
    
}
