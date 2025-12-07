using BoleteHell.Gameplay.Characters.Enemy.Factory;
using BoleteHell.Gameplay.Characters.Registry;
using BoleteHell.Utils.Extensions;
using Pathfinding;
using UnityEngine;
using Zenject;

namespace BoleteHell.Gameplay.SpawnManager
{
    public class SpawnService : ISpawnService, IInitializable
    {
        [Inject]
        private IEntityRegistry _entities;
        
        [Inject]
        private EnemyPool _enemyPool;
        
        [Inject]
        private Camera _camera;
        
        private SpawnArea[] _spawnAreas;

        public void Initialize()
        {
            _spawnAreas = Object.FindObjectsByType<SpawnArea>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        }

        public bool SpawnInArea(SpawnParams parameters)
        {
            if (_spawnAreas.Length == 0)
                return false;
            
            Vector2 targetLocation = _entities.GetPlayer().transform.position;
            SpawnArea spawnArea = _spawnAreas.TakeClosestTo(sa => sa.transform.position, targetLocation, out _);
            Vector2 candidatePos = GetRandomSpawnPosition(spawnArea);
            
            return SpawnAt(parameters with { position = candidatePos });
        }

        public bool SpawnAt(SpawnParams parameters)
        {
            Vector2? navigablePos = FindNearestNavigablePos(parameters.position);
            if (navigablePos == null)
                return false;
            
            _enemyPool.Spawn(parameters.prefab, navigablePos.Value, parameters.groupID);
            return true;
        }

        private static Vector2 GetRandomSpawnPosition(SpawnArea spawnArea)
        {
            Vector2 dir2D = Random.insideUnitCircle.normalized;
            float dist = Random.Range(spawnArea.minSpawnRadius, spawnArea.maxSpawnRadius);
            Vector2 center2D = new Vector2(spawnArea.transform.position.x, spawnArea.transform.position.y);
            return center2D + dir2D * dist;
        }


        private Vector2? FindNearestNavigablePos(Vector2 pos)
        {
            GameObject player = _entities.GetPlayer();
            Debug.Assert(player);
            Debug.Assert(AstarPath.active);
            Debug.Assert(_camera);

            NNInfo nearestNode = AstarPath.active.GetNearest(pos, NNConstraint.Default);
            
            if (nearestNode.node != null)
            {
                return nearestNode.position;
            }

            return null;
        }
    }
}