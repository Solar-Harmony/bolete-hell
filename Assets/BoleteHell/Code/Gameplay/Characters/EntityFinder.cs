using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

namespace BoleteHell.Code.Gameplay.Characters
{
    public class EntityFinder : MonoBehaviour, IEntityFinder
    {
        private Player _player;
        private List<Enemy> _enemies;
        private List<Enemy> _elites;

        public void Awake()
        {
            _player = FindFirstObjectByType<Player>();
            _enemies = new List<Enemy>(FindObjectsByType<Enemy>(FindObjectsSortMode.None));
            _elites = _enemies.FindAll(e => e.isElite);
        }

        public Player GetPlayer()
        {
            return _player;
        }

        // since enemy will be destroyed when he died we can't pass it itself
        public record EnemyDiedEventData(string Name);
        
        [CanBeNull] public event Action<EnemyDiedEventData> EnemyDied;

        public void NotifyEnemyDied(Enemy enemy)
        {
            EnemyDied?.Invoke(new EnemyDiedEventData(enemy.name));
        }

        public List<Enemy> GetAllEnemies()
        {
            return _enemies;
        }

        public void AddEnemy(Enemy enemy)
        {
            _enemies.Add(enemy);
        }

        public void RemoveEnemy(Enemy enemy)
        {
            _enemies.Remove(enemy);
        }

        public Enemy GetWeakestEliteAlive()
        {
            return _elites
                .Where(e => e.Health.CurrentHealth > 0)
                .OrderBy(e => e.Health.CurrentHealth)
                .FirstOrDefault();
        }
    }
}