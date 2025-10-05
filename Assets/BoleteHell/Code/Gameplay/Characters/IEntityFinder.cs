using System.Collections.Generic;

namespace BoleteHell.Code.Gameplay.Characters
{
    public interface IEntityFinder
    {
        Player GetPlayer();
        List<Enemy> GetAllEnemies();
    }
}