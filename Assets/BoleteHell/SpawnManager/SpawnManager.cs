using System.Collections;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public SpawnList spawnList;  //just a spawnlist of biome for now i havent defined how to collide with biome logic
    public SpawnPoint spawnPoints;
    private EnemyData targetEnemy;
    private bool Spawning;

    private void Update()
    {
        if (Spawning)
        {
            // gamba choose enemy and determine weight
            var entries = spawnList.allowedEnemies;
            if (entries == null || entries.Length == 0)
                return;

            SpawnSelectedEnemy(targetEnemy);
        }
    }

    public void SpawnSelectedEnemy(EnemyData targetEnemy)
    {
        // call spawnpoint.cs for help pls uwu
        Vector3 spawnPos = spawnPoints.GetSpawnPosition(targetEnemy);
        GameObject prefabToSpawn = targetEnemy.normalPrefab; //reminder enemy data and spawnlist are scriptable objects not mono so fuck my life

        Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);
    }
}
