using System.Collections;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public SpawnList spawnList;
    public SpawnArea spawnAreaCoordinate;
    private EnemyData targetEnemy;
    private bool Spawning;

    public void TrySpawning()
    {
        if (!Spawning)
            return;

        var entries = spawnList.allowedEnemies;
        if (entries == null || entries.Length == 0)
            return;

        SpawnSelectedEnemy(targetEnemy);
    }

    public Vector2 GetSpawnPosition(EnemyData enemyData)
    {
        Vector2 dir = Random.insideUnitCircle.normalized;
        float dist = Random.Range(spawnAreaCoordinate.minSpawnRadius, (spawnAreaCoordinate.maxSpawnRadius));
        Vector2 spawnPos = transform.position + new Vector3(dir.x * dist, 0f, dir.y * dist);
        return spawnPos;
    }

    public void SpawnSelectedEnemy(EnemyData targetEnemy)
    {
        // call spawnpoint.cs for help pls uwu
        Vector3 spawnPos = GetSpawnPosition(targetEnemy);
        GameObject prefabToSpawn = targetEnemy.normalPrefab; //reminder enemy data and spawnlist are scriptable objects not mono so fuck my life

        Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);
    }
}
