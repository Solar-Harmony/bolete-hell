using UnityEngine;

[CreateAssetMenu(menuName = "AI/SpawnList")]
public class SpawnArea : MonoBehaviour
{
    public GameObject[] allowedEnemies;

    [Tooltip("min distance in radius")]
    public float minSpawnRadius = 5f;
    [Tooltip("max distance in radius")]
    public float maxSpawnRadius = 50f;


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, minSpawnRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, maxSpawnRadius);
    }
}

