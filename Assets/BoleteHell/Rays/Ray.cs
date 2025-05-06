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
        [field: SerializeField]
        public float LightRefractiveIndex { get; private set; }

        [field: SerializeField] public int HitDamage { get; private set; }
        [SerializeReference] private RayHitLogic logic;
        [SerializeField] private int maxNumberOfBounces = 10;
        [SerializeField] private float maxRayDistance = 10;
        private readonly List<Vector3> _rayPositions = new();
        private Vector2 _currentDirection;

        private Vector3 _currentPos;

        //TODO: Find a way to reserve a LineRenderer from the pool until the player stops shooting / has to reload
        public void Cast(Vector3 bulletSpawnPoint, Vector2 direction,InstantRayRenderer lineRenderer)
        {
            _currentPos = bulletSpawnPoint;
            _rayPositions.Add(_currentPos);
            _currentDirection = direction;

            for (int i = 0; i <= maxNumberOfBounces; i++)
            {
                RaycastHit2D hit = Physics2D.Raycast(_currentPos, _currentDirection,maxRayDistance);
                if (!hit)
                {
                    Debug.DrawRay(_currentPos, _currentDirection * 10, Color.black);
                    _rayPositions.Add((Vector2)_currentPos + _currentDirection * 10);
                    break;
                }

                if (hit.transform.gameObject.TryGetComponent(out Line lineHit))
                {
                    Debug.DrawLine(_currentPos, hit.point, Color.blue);
                    _currentDirection = lineHit.OnRayHitLine(_currentDirection, hit, LightRefractiveIndex);
                    _currentPos = hit.point + _currentDirection * 0.01f;
                    _rayPositions.Add(_currentPos);
                    ;
                }
                else if (hit.collider.CompareTag("Enemy"))
                {
                    _rayPositions.Add(hit.point);
                    logic.OnHit();
                }
            }
        
            lineRenderer.DrawRay(_rayPositions,Color);
            _rayPositions.Clear();
        
        }
    }
}