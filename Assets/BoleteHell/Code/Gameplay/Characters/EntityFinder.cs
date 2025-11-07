using System.Collections.Generic;
using System.Linq;
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

        public List<Enemy> GetAllEnemies()
        {
            return _enemies;
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