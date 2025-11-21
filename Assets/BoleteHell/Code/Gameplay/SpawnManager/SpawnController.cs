using System;
using System.Collections;
using System.Collections.Generic;
using BoleteHell.Code.Gameplay.Base;
using BoleteHell.Code.Gameplay.Characters;
using BoleteHell.Code.Utils;
using UnityEngine;
using Zenject;

namespace BoleteHell.Code.Gameplay.SpawnManager
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
        private IEntityFinder _entities;

        [Inject]
        private IBaseService _bases;
        
        private List<SpawnArea> _spawnAreas;
        
        private Coroutine _spawnCoroutine = null;

        public SpawnTargetPriority _goal; // TEMP, will be modified by the AI director
        
        private void Start()
        {
            _spawnAreas = new List<SpawnArea>(FindObjectsByType<SpawnArea>(FindObjectsSortMode.None));
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
                Vector2 targetLocation = FindTargetLocation();
                SpawnArea spawnArea = FindClosestSpawnArea(targetLocation);
            
                if (spawnArea)
                {
                    _spawnManager.Spawn(spawnArea);
                }

                yield return new WaitForSeconds(5.0f);
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
                SpawnTargetPriority.Player => _entities.GetPlayer() ? _entities.GetPlayer().Position : Vector2.zero,
                SpawnTargetPriority.WeakestPlayerBase => _bases.GetWeakestBase().Position,
                SpawnTargetPriority.DefendWeakestElites => _entities.GetWeakestEliteAlive()?.Position ?? _entities.GetPlayer().Position,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}