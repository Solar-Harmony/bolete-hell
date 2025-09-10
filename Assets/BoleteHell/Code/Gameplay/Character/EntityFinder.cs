using System.Collections.Generic;
using UnityEngine;

namespace BoleteHell.Code.Gameplay.Character
{
    public class EntityFinder : MonoBehaviour, IEntityFinder
    {
        private Player _player;
        private List<Enemy> _enemies;

        public void Awake()
        {
            _player = FindFirstObjectByType<Player>();
            _enemies = new List<Enemy>(FindObjectsByType<Enemy>(FindObjectsSortMode.None));
        }

        public Player GetPlayer()
        {
            return _player;
        }

        public List<Enemy> GetAllEnemies()
        {
            return _enemies;
        }
    }
}