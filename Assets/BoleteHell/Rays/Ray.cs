using UnityEngine;

[CreateAssetMenu(fileName = "Ray", menuName = "Scriptable Objects/Ray/RayInfo")]

public class Ray:ScriptableObject
{
    [field: SerializeField] public Color color { get; private set; }
    [field: SerializeField] public float lightRefractiveIndice { get; private set; }
    [field: SerializeField] public int hitDamage { get; private set; }
    //ray
    [SerializeReference]private RayHitLogic logic;


    public void Cast(Vector3 bulletSpawnPoint, Vector3 direction)
    {
        RaycastHit2D hit = Physics2D.Raycast(bulletSpawnPoint, direction,10 );

        //TODO: change raycasts to the actual way of shooting lasers and fix the problems it give with shield collision
        if (hit)
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
            //Si touche un ennemi call on hit
        }
        else
        {
            Debug.DrawRay(bulletSpawnPoint, direction * 10, Color.black);
        }
    }

    public void OnHit()
    {
        logic.OnHit();
    }
}