using System.Collections.Generic;
using BoleteHell.Code.Arsenal.Cannons;

namespace BoleteHell.Code.Arsenal.ShotPatterns
{
    public interface IShotPatternService
    {
        List<ShotLaunchParams> ComputeSpawnPoints(ShotPatternData pattern, ShotLaunchParams parameters, int shotCount);
    }
}