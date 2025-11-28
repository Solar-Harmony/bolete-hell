using System;
using System.Collections;
using System.Collections.Generic;
using BoleteHell.Gameplay.Characters.Registry;
using BoleteHell.Utils.Extensions;
using UnityEngine;
using Zenject;

namespace BoleteHell.Gameplay.SpawnManager
{
    public enum SpawnTargetPriority
    {
        Player,
        WeakestPlayerBase,
        DefendWeakestElites
    }
    
    public class SpawnController : MonoBehaviour
    {
        [Inject]
        private SpawnManager _spawnManager;
        
        [Inject]
        private IEntityRegistry _entities;

        [Serializable]
        public class Config
        {
            public bool AllowSpawning = true;
            public float Interval = 5.0f;
            public int MaxEnemies = 7;
        }

        [Inject]
        private Config _config;
        
        private List<SpawnArea> _spawnAreas;

        private int _enemiesSpawned = 0;
        
        private Coroutine _spawnCoroutine = null;

        public SpawnTargetPriority _goal; // TEMP, will be modified by the AI director
        
        private void Start()
        {
            _spawnAreas = new List<SpawnArea>(FindObjectsByType<SpawnArea>(FindObjectsSortMode.None));

            _entities.EntityDied += _ => _enemiesSpawned--; 
            
            if (_config.AllowSpawning)
                StartSpawning();
        }
        
        public void StartSpawning()
        {
            _spawnCoroutine ??= StartCoroutine(nameof(SpawnEnemies));
        }
        
        public void StopSpawning()
        {
            if (_spawnCoroutine != null)
            {
                StopCoroutine(_spawnCoroutine);
                _spawnCoroutine = null;
            }
        }

        private IEnumerator SpawnEnemies()
        {
            while (true)
            {
                if (_enemiesSpawned < _config.MaxEnemies)
                {
                    Vector2 targetLocation = FindTargetLocation();
                    SpawnArea spawnArea = FindClosestSpawnArea(targetLocation);
            
                    if (spawnArea)
                    {
                        _spawnManager.Spawn(spawnArea);
                        _enemiesSpawned++;
                    }
                }

                yield return new WaitForSeconds(_config.Interval);
            }
        }

        private SpawnArea FindClosestSpawnArea(Vector2 location)
        {
            return _spawnAreas.FindClosestTo(sa => sa.transform.position, location, out _);
        }

        private Vector2 FindTargetLocation()
        {
            return _goal switch
            {
                SpawnTargetPriority.Player => _entities.GetPlayer().transform.position,
                SpawnTargetPriority.WeakestPlayerBase => _entities.GetWeakestBase().transform.position,
                SpawnTargetPriority.DefendWeakestElites => _entities.GetWeakestEliteAlive()?.transform.position ?? _entities.GetPlayer().transform.position,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}