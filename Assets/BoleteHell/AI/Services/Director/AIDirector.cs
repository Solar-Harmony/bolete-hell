using System;
using BoleteHell.AI.Services.Group;
using BoleteHell.Gameplay.Characters.Enemy;
using BoleteHell.Gameplay.Characters.Registry;
using BoleteHell.Gameplay.GameState;
using BoleteHell.Gameplay.SpawnManager;
using BoleteHell.Utils.Extensions;
using BoleteHell.Utils.LogFilter;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace BoleteHell.AI.Services
{
    public class AIDirector : MonoBehaviour
    {
        [Inject]
        private IEntityRegistry _entities;

        [Inject]
        private ISpawnService _spawner;
        
        [Inject]
        private IAIGroupService _groupService;

        [Inject]
        private IGameOutcomeService _outcome;
        
        [Inject]
        private Config _config;

        private double _money;
        
        private AIGroup _attackPlayerGroup;

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
            _attackPlayerGroup = _groupService.CreateGroup();
            _attackPlayerGroup.Target = _entities.GetPlayer();
            _outcome.OnVictory += () => CancelInvoke(nameof(Tick));
            _outcome.OnDefeat += _ => CancelInvoke(nameof(Tick));
            
            InvokeRepeating(nameof(Tick), _config.TickInterval, _config.TickInterval);
        }

        private void Update()
        {
            _money += _config.MoneyPerSecond * Time.deltaTime;
        }
        
        private void Tick()
        {
            TryDirectEnemyToBase();
            TrySpawnEnemy();
        }

        private void TryDirectEnemyToBase()
        {
            if (Random.value < 0.1f) 
                return;
            
            var enemy = _entities.GetRandom(EntityTag.Enemy);
            if (!enemy) 
                return;
            
            GameObject nearestBase = _entities.GetClosestBase(enemy.transform.position, out _);
            enemy.GetComponent<AIGroupComponent>().TargetOverride = nearestBase;
        }

        private void TrySpawnEnemy()
        {
            int numEnemies = _entities.GetCount(EntityTag.Enemy);
            if (numEnemies >= _config.MaxConcurrentEnemies)
                return;

            if (IsInBossFight())
                return;

            int rand = Random.Range(0, _config.CostTable.Entries.Length - 1);
            var enemyToSpawn = _config.CostTable.Entries[rand];
            
            // TODO: The cost logic is not good as it is, we would need to complexify it to make it better
            // var enemyToSpawn = _config.CostTable.Entries
            //     .Where(e => e.Cost <= _money)
            //     .WithHighest(e => e.Cost);
            
            Vector2 playerPos = _entities.GetPlayer().transform.position;

            if (_spawner.SpawnInArea(new SpawnParams(enemyToSpawn.Prefab, playerPos, _attackPlayerGroup.GroupID)))
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

        private void OnGUI()
        {
            string text = $"AI Money: {_money:F1}$";
            GUIStyle style = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.UpperRight
            };

            Vector2 size = style.CalcSize(new GUIContent(text));
            Rect rect = new Rect(Screen.width - size.x - 10, 10, size.x, size.y);

            GUI.Label(rect, text, style);
        }
    }
}