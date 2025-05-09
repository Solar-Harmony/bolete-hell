using System.Collections.Generic;
using Lasers.RayLogic;
using Shields;
using UnityEngine;

namespace Lasers
{
    [CreateAssetMenu(fileName = "Ray", menuName = "Scriptable Objects/Ray/RayInfo")]
    public class Ray : ScriptableObject
    {
        [field: SerializeField] public Color Color { get; private set; }

        [Tooltip("La valeur ne devrait pas dépasser la valeur de refraction du shield sinon ça créé des problèmes")]
        [field: SerializeField] public float LightRefractiveIndex { get; private set; }

        [field: SerializeReference] public RayHitLogic logic { get; private set; }
        [field:SerializeField] public int maxNumberOfBounces { get; private set; } = 10;
        [field:SerializeField] public float maxRayDistance { get; private set; } = 10;
        [field:SerializeField] public float rayWidth { get; private set; } = 0.2f;
        [Header("Specific to projectile lasers")]
        [field:SerializeField] public float raylength { get; private set; } = 0.3f;
        


        //TODO: Find a way to reserve a LineRenderer from the pool until the player stops shooting / has to reload
        //Pourrait ajouter un param pour override le maxRayDistance pour quand on tire des petit lasers
        //Donc dans le firing log de petit laser on appel
        // public void Cast(Vector3 bulletSpawnPoint, Vector2 direction,LaserRenderer lineRenderer)
        // {
        //     _currentPos = bulletSpawnPoint;
        //     _rayPositions.Add(_currentPos);
        //     _currentDirection = direction;
        //
        //     for (int i = 0; i <= maxNumberOfBounces; i++)
        //     {
        //         RaycastHit2D hit = Physics2D.Raycast(_currentPos, _currentDirection,maxRayDistance);
        //         if (!hit)
        //         {
        //             Debug.DrawRay(_currentPos, _currentDirection * maxRayDistance, Color.black);
        //             _rayPositions.Add((Vector2)_currentPos + _currentDirection * maxRayDistance);
        //             break;
        //         }
        //
        //         if (hit.transform.gameObject.TryGetComponent(out Line lineHit))
        //         {
        //             Debug.DrawLine(_currentPos, hit.point, Color.blue);
        //             _currentDirection = lineHit.OnRayHitLine(_currentDirection, hit, LightRefractiveIndex);
        //             _currentPos = hit.point + _currentDirection * 0.01f;
        //             _rayPositions.Add(_currentPos);
        //         }
        //         else if (hit.collider.CompareTag("Enemy"))
        //         {
        //             _rayPositions.Add(hit.point);
        //             logic.OnHit();
        //         }
        //     }
        //
        //     lineRenderer.DrawRay(_rayPositions,Color);
        //     _rayPositions.Clear();
        // }
    }
}