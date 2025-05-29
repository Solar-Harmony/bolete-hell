using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    public SpawnList spawnList;
    [Tooltip("min distance in radius")]
    public float minSpawnRadius = 5f;
    [Tooltip("max distance in radius")]
    public float maxSpawnRadius = 50f;

    public Vector2 GetSpawnPosition(EnemyData enemyData)
    {
        Vector2 dir = Random.insideUnitCircle.normalized;
        float dist = Random.Range(minSpawnRadius, maxSpawnRadius);
        Vector2 spawnPos = transform.position + new Vector3(dir.x * dist, 0f, dir.y * dist);
        return spawnPos;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, minSpawnRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, maxSpawnRadius);
    }
}

