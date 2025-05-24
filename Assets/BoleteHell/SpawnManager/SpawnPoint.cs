using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    public SpawnList spawnList;

    private Transform player;

    private void Awake()
    {
        player = GameObject.FindWithTag("Player").transform; //must be changed if not using tag system
    }

    public Vector3 GetSpawnPosition(EnemyData enemyData)
    {
        Vector2 unitCircle = Random.insideUnitCircle.normalized;
        float distance = Random.Range(enemyData.minSpawnRadius, enemyData.maxSpawnRadius);
        Vector3 offset = new Vector3(unitCircle.x * distance, 0f, unitCircle.y * distance);

        Vector3 SpawnPos = player.position + offset;
        return SpawnPos;
    }
}
