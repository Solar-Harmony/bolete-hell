using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace BoleteHell.Code.Gameplay.Characters
{
    public interface IEntityFinder
    {
        Player GetPlayer();
        
        event Action<EntityFinder.EnemyDiedEventData> EnemyDied;
        void NotifyEnemyDied(Enemy enemy);
        
        List<Enemy> GetAllEnemies();
        void AddEnemy(Enemy enemy);
        void RemoveEnemy(Enemy enemy);
        [CanBeNull] Enemy GetWeakestEliteAlive();
    }
}