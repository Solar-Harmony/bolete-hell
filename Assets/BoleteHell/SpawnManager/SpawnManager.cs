using System.Collections;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public SpawnList spawnList;  //just a spawnlist of biome for now i havent defined how to collide with biome logic
    public SpawnPoint spawnPoints;
    public float weightGainPerSecond = 1f;
    private float currentWeight = 0f;
    private EnemyData targetEnemy;

    private void Start()
    {
        StartCoroutine(SpawnCycle());
    }

    private IEnumerator SpawnCycle()
    {
        while (true)
        {
            // gamba choose enemy and determine weight
            var entries = spawnList.allowedEnemies;
            if (entries == null || entries.Length == 0)
                yield break;

            targetEnemy = entries[Random.Range(0, entries.Length)]; //gamba enemy
            float cost = targetEnemy.spawnWeight; //find weight

            // Accumulate weight until we can afford it
            while (currentWeight < cost)
            {
                currentWeight += weightGainPerSecond * Time.deltaTime;
                yield return null;
            }

            // spawn the target enemy
            SpawnSelectedEnemy(targetEnemy);

            // Pay the money lol
            currentWeight -= cost;
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
