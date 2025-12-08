using System.Collections.Generic;
using BoleteHell.Code.Arsenal.Cannons;
using UnityEngine;

namespace BoleteHell.Code.Arsenal.ShotPatterns
{
    public class ShotPatternService : IShotPatternService
    {
        public List<ShotLaunchParams> ComputeSpawnPoints(ShotPatternData pattern, ShotLaunchParams parameters, int shotCount)
        {
            float spawnDistance = Vector3.Distance(parameters.CenterPos, parameters.SpawnPosition);
            
            int maxSideAngle = pattern.maxAngleRange / 2;
            var projectileData = new List<ShotLaunchParams>();
            
            for (int i = 0; i < pattern.numberOfBulletShot; i++)
            {
                float currentAngle = SetProjectileAngle(pattern.numberOfBulletShot, i, maxSideAngle);
                projectileData.Add(ApplyPatternTransform(pattern, currentAngle, spawnDistance, parameters, shotCount));
            }

            return projectileData;
        }
        
        private float SetProjectileAngle(int numberOfBullets, int index, int maxSideAngle)
        {
            if (numberOfBullets == 1)
            {
                return 0f;
            }

            // Interpolation entre le point le plus a gauche et le point le plus a droite
            float t = (float)index / numberOfBullets;
            return Mathf.Lerp(-maxSideAngle, maxSideAngle, t);
        }

        private ShotLaunchParams ApplyPatternTransform(ShotPatternData pattern, float currentAngle, float spawnDistance, ShotLaunchParams parameters, int shotCount)
        {
            Quaternion rotation2D = Quaternion.Euler(0, 0, currentAngle + pattern.startingRotation + (shotCount * pattern.constantRotation));
            
            Vector2 direction = rotation2D * (pattern.followOwnerLookingDirection ? parameters.SpawnDirection : Vector2.right);
            Vector2 spawnPosition = parameters.CenterPos + (direction.normalized * spawnDistance);
            
            return parameters with { SpawnDirection = direction, SpawnPosition = spawnPosition };
        }
    }
}
