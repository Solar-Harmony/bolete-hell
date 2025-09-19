using System.Collections.Generic;

namespace BoleteHell.Code.Gameplay.Character
{
    public interface IEntityFinder
    {
        Player GetPlayer();
        List<Enemy> GetAllEnemies();
    }
}