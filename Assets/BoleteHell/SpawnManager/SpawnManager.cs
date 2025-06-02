using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public SpawnArea spawnArea;

    public void TrySpawning(Transform spawnPoint) 
    {
        var entries = spawnArea.spawnListEnemy.allowedEnemies;
        if (entries == null || entries.Length == 0)
            return;

        SpawnSelectedEnemy(spawnArea.spawnListEnemy, spawnPoint);
    }

    public Vector2 GetSpawnPosition(Transform centerPoint)
    {
        Vector2 dir2D = Random.insideUnitCircle.normalized;

        float dist = Random.Range(
            spawnArea.minSpawnRadius,
            spawnArea.maxSpawnRadius
        );

        Vector2 offset2D = dir2D * dist;
        Vector2 center2D = new Vector2(centerPoint.position.x, centerPoint.position.y);

        Vector2 fml = new Vector2(center2D.x + offset2D.x,center2D.y + offset2D.y);
        return fml;
    }

    public void SpawnSelectedEnemy(SpawnListEnemy allowedEnemies, Transform spawnPoint)
    {
        Vector3 finalSpawnPos = GetSpawnPosition(spawnPoint);
        int index = Random.Range(0, allowedEnemies.allowedEnemies.Length);
        GameObject prefabToSpawn = allowedEnemies.allowedEnemies[index];

        Instantiate(prefabToSpawn, finalSpawnPos, Quaternion.identity);
    }
}