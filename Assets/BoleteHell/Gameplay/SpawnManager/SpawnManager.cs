using BoleteHell.Gameplay.Characters.Registry;
using UnityEngine;
using Zenject;

namespace BoleteHell.Gameplay.SpawnManager
{
    public class SpawnManager : MonoBehaviour
    {
        [Inject]
        private DiContainer _container; // TODO: Temporary, we should make a simple factory

        [Inject]
        private IEntityRegistry _entityRegistry;

        private int counter;
        public bool Spawn(SpawnArea spawnArea)
        {
            if (!spawnArea.spawnList || spawnArea.spawnList.allowedEnemies.Length == 0)
            {
                Debug.LogWarning("Tried to spawn from an empty spawnlist. Aborting spawn.");
                return false;
            }
            
            GameObject[] entries = spawnArea.spawnList.allowedEnemies;
            if (entries == null || entries.Length == 0)
                return false;

            SpawnSelectedEnemy(spawnArea.spawnList, spawnArea);
            return true;
        }

        private static Vector2 GetSpawnPosition(SpawnArea spawnArea)
        {
            Vector2 dir2D = Random.insideUnitCircle.normalized;
            float dist = Random.Range(spawnArea.minSpawnRadius, spawnArea.maxSpawnRadius);
            Vector2 center2D = new Vector2(spawnArea.transform.position.x, spawnArea.transform.position.y);
            return center2D + dir2D * dist;
        }

        private void SpawnSelectedEnemy(SpawnList allowedEnemies, SpawnArea spawnArea)
        {
            int index = Random.Range(0, allowedEnemies.allowedEnemies.Length);
            GameObject prefabToSpawn = allowedEnemies.allowedEnemies[index];

            GameObjectCreationParameters parameters = new()
            {
                Position = GetSpawnPosition(spawnArea)
            };
            
            GameObject enemy = _container.InstantiatePrefab(prefabToSpawn, parameters);
            enemy.transform.name = enemy.name + $"{counter}";
            _entityRegistry.Register(new []{ EntityTag.Enemy }, enemy);
            counter++;
        }
    }
}