using BoleteHell.Gameplay.Characters.Enemy;
using BoleteHell.Gameplay.Characters.Registry;
using UnityEngine;
using Zenject;

namespace BoleteHell.Gameplay.SpawnManager
{
    public class SpawnManager : MonoBehaviour, ISpawnService
    {
        [Inject]
        private DiContainer _container; // TODO: Temporary, we should make a simple factory

        [Inject]
        private IEntityRegistry _entities;

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
            
            GameObjectCreationParameters parameters = new()
            {
                Position = position
            };

            foreach (var prefabToSpawn in list.allowedEnemies)
            {
                GameObject enemy = _container.InstantiatePrefab(prefabToSpawn, parameters);
                enemy.transform.name = enemy.name + $"{_counter}";
                enemy.GetComponent<AIGroupComponent>().GroupID = groupID;
                
                _entities.Register(new []{ EntityTag.Enemy }, enemy);
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
            
            GameObjectCreationParameters parameters = new()
            {
                Position = position
            };

            foreach (var prefabToSpawn in list.allowedEnemies)
            {
                GameObject enemy = _container.InstantiatePrefab(prefabToSpawn, parameters);
                enemy.transform.name = enemy.name + $"{_counter}";
                _entities.Register(new []{ EntityTag.Enemy }, enemy);
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

            GameObjectCreationParameters parameters = new()
            {
                Position = GetSpawnPosition(spawnArea)
            };
            
            GameObject enemy = _container.InstantiatePrefab(prefabToSpawn, parameters);
            enemy.transform.name = enemy.name + $"{_counter}";
            _entities.Register(new []{ EntityTag.Enemy }, enemy);
            _counter++;
        }
    }
}