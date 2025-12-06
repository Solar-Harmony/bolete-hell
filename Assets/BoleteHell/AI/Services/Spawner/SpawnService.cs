using BoleteHell.Gameplay.Characters.Enemy.Factory;
using BoleteHell.Gameplay.Characters.Registry;
using UnityEngine;
using Zenject;

namespace BoleteHell.Gameplay.SpawnManager
{
    public class SpawnService : ISpawnService
    {
        [Inject]
        private IEntityRegistry _entities;
        
        [Inject]
        private EnemyPool _enemyPool;

        private int _counter;
        
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

        public bool Spawn(SpawnList list, Vector2 position, int groupID)
        {
            if (!list || list.allowedEnemies.Length == 0)
            {
                Debug.LogWarning("Tried to spawn from an empty spawnlist. Aborting spawn.");
                return false;
            }

            foreach (var prefabToSpawn in list.allowedEnemies)
            {
                _enemyPool.Spawn(prefabToSpawn, position, groupID);
                _entities.Register(new []{ EntityTag.Enemy }, prefabToSpawn);
                _counter++;
            }
            
            return true;
        }

        public bool Spawn(SpawnList list, Vector2 position)
        {
            if (!list || list.allowedEnemies.Length == 0)
            {
                Debug.LogWarning("Tried to spawn from an empty spawnlist. Aborting spawn.");
                return false;
            }

            foreach (var prefabToSpawn in list.allowedEnemies)
            {
                EnemyPoolable enemy = _enemyPool.Spawn(prefabToSpawn, position);
                _entities.Register(new []{ EntityTag.Enemy }, enemy.gameObject);
                _counter++;
            }
            
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
            
            EnemyPoolable enemy = _enemyPool.Spawn(prefabToSpawn,  GetSpawnPosition(spawnArea));
            _entities.Register(new []{ EntityTag.Enemy }, enemy.gameObject);
            _counter++;
        }
    }
}