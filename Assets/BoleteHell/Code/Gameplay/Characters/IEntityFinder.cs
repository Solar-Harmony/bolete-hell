using System.Collections.Generic;
using JetBrains.Annotations;

namespace BoleteHell.Code.Gameplay.Characters
{
    public interface IEntityFinder
    {
        Player GetPlayer();
        List<Enemy> GetAllEnemies();
        [CanBeNull] Enemy GetWeakestEliteAlive();
    }
}