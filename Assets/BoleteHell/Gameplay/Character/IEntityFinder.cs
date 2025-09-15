using System.Collections.Generic;

namespace BoleteHell.Gameplay.Character
{
    public interface IEntityFinder
    {
        Player GetPlayer();
        List<Enemy> GetAllEnemies();
    }
}