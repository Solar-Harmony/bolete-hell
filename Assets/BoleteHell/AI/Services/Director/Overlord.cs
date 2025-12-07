using System;
using System.Linq;
using BoleteHell.AI.Services.Group;
using BoleteHell.Gameplay.Characters.Registry;
using BoleteHell.Gameplay.SpawnManager;
using BoleteHell.Utils.Extensions;
using BoleteHell.Utils.LogFilter;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace BoleteHell.AI.Services
{
    public class Overlord : MonoBehaviour
    {
        [Inject]
        private IEntityRegistry _entities;

        [Inject]
        private ISpawnService _spawner;
        
        [Inject]
        private IAIGroupService _groupService;
        
        [Inject]
        private Config _config;

        private double _money;
        private int _groupID;

        private static readonly LogCategory _logAIController = new("AI Controller", Color.red);

        [Serializable]
        public class Config
        {
            [Required]
            public CostTable CostTable;
            public float TickInterval = 5.0f;
            public int MaxConcurrentEnemies = 10;
            public int InitialMoney = 1000;
            public int MoneyPerSecond = 10;
            
            [Unit(Units.Meter)] 
            [Tooltip("Distance from an elite where we should stop random spawns.")]
            public float BossFightDistance = 25.0f;
        }
        
        private void Start()
        {
            _money = _config.InitialMoney;
            _groupID = _groupService.CreateGroup();
            InvokeRepeating(nameof(Tick), _config.TickInterval, _config.TickInterval);
        }

        private void Update()
        {
            _money += _config.MoneyPerSecond * Time.deltaTime;
        }

        /*
         * logique:
         * - spawn from the nearest spawner
         * - pick the most expensive enemy
         * - if in a boss fight (near boss), stop the spawning
         */
        private void Tick()
        {
            int numEnemies = _entities.GetCount(EntityTag.Enemy);
            if (numEnemies >= _config.MaxConcurrentEnemies)
                return;

            if (IsInBossFight())
                return;

            var enemyToSpawn = _config.CostTable.Entries
                .Where(e => e.Cost <= _money)
                .WithHighest(e => e.Cost);
            
            Vector2 playerPos = _entities.GetPlayer().transform.position;

            if (_spawner.SpawnInArea(new SpawnParams(enemyToSpawn.Prefab, playerPos, _groupID)))
            {
                Scribe.Log(_logAIController, "Spawned '{0}' for {1}$.", enemyToSpawn.Prefab.name, enemyToSpawn.Cost);
            }
        }

        private bool IsInBossFight()
        {
            _entities
                .WithTag(EntityTag.EliteEnemy)
                .TakeClosestTo(_entities.GetPlayer(), out float distance);

            return distance <= _config.BossFightDistance;
        }
    }
}