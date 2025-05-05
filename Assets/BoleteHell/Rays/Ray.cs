using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ray", menuName = "Scriptable Objects/Ray/RayInfo")]

public class Ray:ScriptableObject
{
    [field: SerializeField] public Color color { get; private set; }
    [Tooltip("La valeur ne devrait pas dépasser la valeur de refraction du shield sinon ça créé des problèmes")]
    [field: SerializeField] public float lightRefractiveIndice { get; private set; }
    [field: SerializeField] public int hitDamage { get; private set; }
    [SerializeReference]private RayHitLogic logic;

    [SerializeField] private int maxNumberOfBounces = 10;

    [SerializeField] private float maxRayDistance = 10;

    private Vector3 currentPos;
    private Vector2 currentDirection;
    private List<Vector3> rayPositions = new ();

    //TODO: Find a way to reserve a LineRenderer from the pool until the player stops shooting / has to reload 
    public void Cast(Vector3 bulletSpawnPoint, Vector2 direction,InstantRayRenderer lineRenderer)
    {
        currentPos = bulletSpawnPoint;
        rayPositions.Add(currentPos);
        currentDirection = direction;

        for (int i = 0; i <= maxNumberOfBounces; i++)
        {
            RaycastHit2D hit = Physics2D.Raycast(currentPos, currentDirection,maxRayDistance);
            if (hit)
            {
                if (hit.transform.gameObject.TryGetComponent(out Line lineHit))
                {
                    Debug.DrawLine(currentPos,hit.point,Color.blue);
                    currentDirection = lineHit.OnRayHitLine(currentDirection, hit,lightRefractiveIndice);
                    currentPos = hit.point + currentDirection * 0.01f;
                    rayPositions.Add(currentPos); ;

                }
                else if (hit.transform.gameObject.TryGetComponent(out Enemy enemy))
                {
                    rayPositions.Add(hit.point);
                    logic.OnHit();
                }
            }
            else
            {
                Debug.DrawRay(currentPos,currentDirection * 10,Color.black);
                rayPositions.Add((Vector2)currentPos + currentDirection * 10);
                break;
            }
        }
        
        lineRenderer.DrawRay(rayPositions,color);
        rayPositions.Clear();
        
    }

    public void OnHit()
    {
        logic.OnHit();
    }
}