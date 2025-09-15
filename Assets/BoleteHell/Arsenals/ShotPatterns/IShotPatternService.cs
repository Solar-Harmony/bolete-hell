using System.Collections.Generic;
using BoleteHell.Arsenals.Cannons;

namespace BoleteHell.Arsenals.ShotPatterns
{
    public interface IShotPatternService
    {
        List<ShotLaunchParams> ComputeSpawnPoints(ShotPatternData pattern, ShotLaunchParams parameters, int shotCount);
    }
}