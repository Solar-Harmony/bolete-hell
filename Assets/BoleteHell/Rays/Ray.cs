using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public abstract class Ray
{
    [field: SerializeField] public Color color { get; private set; }
    [field: SerializeField] public float lightRefractiveIndice { get; private set; }

    private float windUpTime;
    private float reloadTime;


    public void Cast(Vector3 bulletSpawnPoint, Vector3 direction)
    {
        RaycastHit hit;

        if (Physics.Raycast(bulletSpawnPoint, direction, out hit, math.INFINITY))
        {
            if (hit.transform.gameObject.TryGetComponent(out Line lineHit))
            {
                Debug.DrawLine(bulletSpawnPoint, hit.point, color);
                lineHit.OnRayHitLine(direction, hit, this);
            }
            else
            {
                Debug.DrawRay(bulletSpawnPoint, direction * 10,Color.black);
            }
        }
        else
        {
            Debug.DrawRay(bulletSpawnPoint, direction * 10, Color.black);
        }
    }
}