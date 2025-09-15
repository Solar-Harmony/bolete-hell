using UnityEngine;

namespace BoleteHell.Gameplay.SpawnManager
{
    public class SpawnManager : MonoBehaviour
    {

        public bool Spawn(SpawnArea spawnArea)
        {
            var entries = spawnArea.spawnList.allowedEnemies;
            if (entries == null || entries.Length == 0)
                return false;

            SpawnSelectedEnemy(spawnArea.spawnList, spawnArea);
            return true;
        }

        public Vector2 GetSpawnPosition(SpawnArea spawnArea)
        {
            Vector2 dir2D = Random.insideUnitCircle.normalized;
            float dist = Random.Range(spawnArea.minSpawnRadius, spawnArea.maxSpawnRadius);
            Vector2 center2D = new Vector2(spawnArea.transform.position.x, spawnArea.transform.position.y);
            return center2D + dir2D * dist;
        }

        public void SpawnSelectedEnemy(SpawnList allowedEnemies, SpawnArea spawnArea)
        {
            Vector3 finalSpawnPos = GetSpawnPosition(spawnArea);
            int index = Random.Range(0, allowedEnemies.allowedEnemies.Length);
            GameObject prefabToSpawn = allowedEnemies.allowedEnemies[index];

            Instantiate(prefabToSpawn, finalSpawnPos, Quaternion.identity);
        }
    }
}