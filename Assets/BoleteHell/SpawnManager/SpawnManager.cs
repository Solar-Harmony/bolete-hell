using System.Collections;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public SpawnArea spawnAreaCoordinate;

    public void TrySpawning()
    {

        var entries = spawnAreaCoordinate.allowedEnemies;
        if (entries == null || entries.Length == 0)
            return;

        SpawnSelectedEnemy(spawnAreaCoordinate);
    }

    public Vector2 GetSpawnPosition(SpawnArea allowedEnemies)
    {
        Vector2 dir = Random.insideUnitCircle.normalized;
        float dist = Random.Range(spawnAreaCoordinate.minSpawnRadius, (spawnAreaCoordinate.maxSpawnRadius));
        Vector2 spawnPos = transform.position + new Vector3(dir.x * dist, 0f, dir.y * dist);
        return spawnPos;
    }

    public void SpawnSelectedEnemy(SpawnArea allowedEnemies)
    {
        Vector3 spawnPos = GetSpawnPosition(allowedEnemies);
        int fml = Random.Range(0, spawnAreaCoordinate.allowedEnemies.Length);
        GameObject prefabToSpawn = spawnAreaCoordinate.allowedEnemies[fml];
        Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);
    }
}
