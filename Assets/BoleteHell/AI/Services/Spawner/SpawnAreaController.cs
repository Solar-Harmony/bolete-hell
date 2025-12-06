using System;
using System.Collections;
using System.Collections.Generic;
using BoleteHell.Gameplay.Characters.Registry;
using BoleteHell.Gameplay.GameState;
using BoleteHell.Utils.Extensions;
using UnityEngine;
using Zenject;

namespace BoleteHell.Gameplay.SpawnManager
{
    public class SpawnAreaController : MonoBehaviour
    {
        [Inject]
        private ISpawnService _spawnManager;
        
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

        [Inject]
        private IGameOutcomeService _outcome;
        
        private List<SpawnArea> _spawnAreas;

        private int _enemiesSpawned = 0;
        
        private Coroutine _spawnCoroutine = null;
        
        private void Start()
        {
            _spawnAreas = new List<SpawnArea>(FindObjectsByType<SpawnArea>(FindObjectsSortMode.None));

            _entities.EntityDied += _ => _enemiesSpawned--;
            
            _outcome.OnVictory += StopSpawning;
            _outcome.OnDefeat += _ => StopSpawning();
            
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
                    Vector2 targetLocation = _entities.GetPlayer().transform.position;
                    SpawnArea spawnArea = _spawnAreas.TakeClosestTo(sa => sa.transform.position, targetLocation, out _);
            
                    if (spawnArea)
                    {
                        _spawnManager.Spawn(spawnArea);
                        _enemiesSpawned++;
                    }
                }

                yield return new WaitForSeconds(_config.Interval);
            }
        }
    }
}