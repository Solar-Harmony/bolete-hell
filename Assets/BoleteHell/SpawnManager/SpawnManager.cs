using UnityEngine;

public class SpawnManager : MonoBehaviour
{

    public bool Spawn(SpawnArea spawnArea, Transform spawnPoint)
    {
        var entries = spawnArea.spawnList.allowedEnemies;
        if (entries == null || entries.Length == 0)
            return false;

        SpawnSelectedEnemy(spawnArea.spawnList, spawnPoint, spawnArea);
        return true;
    }

    public Vector2 GetSpawnPosition(SpawnArea spawnArea, Transform spawnPoint)
    {
        Vector2 dir2D = Random.insideUnitCircle.normalized;

        float dist = Random.Range(spawnArea.minSpawnRadius,spawnArea.maxSpawnRadius);

        Vector2 offset2D = dir2D * dist;
        Vector2 center2D = new Vector2(spawnPoint.position.x, spawnPoint.position.y);

        Vector2 fml = new Vector2(center2D.x + offset2D.x,center2D.y + offset2D.y);
        return fml;
    }

    public void SpawnSelectedEnemy(SpawnList allowedEnemies, Transform spawnPoint, SpawnArea spawnArea)
    {
        Vector3 finalSpawnPos = GetSpawnPosition(spawnArea, spawnPoint);
        int index = Random.Range(0, allowedEnemies.allowedEnemies.Length);
        GameObject prefabToSpawn = allowedEnemies.allowedEnemies[index];

        Instantiate(prefabToSpawn, finalSpawnPos, Quaternion.identity);
    }
}