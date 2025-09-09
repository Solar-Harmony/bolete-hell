using System.Collections.Generic;
using BoleteHell.Code.Arsenal.Cannons;
using UnityEngine;

namespace BoleteHell.Code.Arsenal.ShotPatterns
{
    public record ProjectileLaunchData(Vector2 StartPosition, Vector2 Direction);

    public class ShotPattern
    {
        public List<ProjectileLaunchData> Fire(ShotPatternData pattern, ShotParams parameters, int shotCount)
        {
            float spawnDistance = parameters.Instigator
                ? Vector3.Distance(parameters.Instigator.transform.position, parameters.SpawnPosition)
                : 0.0f;
            
            int maxSideAngle = pattern.maxAngleRange / 2;
            var projectileData = new List<ProjectileLaunchData>();
            
            for (int i = 0; i < pattern.numberOfBulletShot; i++)
            {
                float currentAngle = SetBulletAngle(pattern.numberOfBulletShot, i, maxSideAngle);
                ShotParams launchParams = SetupBulletPosition(pattern, currentAngle, spawnDistance, parameters, shotCount);
                projectileData.Add(new ProjectileLaunchData(launchParams.SpawnPosition, launchParams.SpawnDirection));
            }

            return projectileData;
        }
        
        private static float SetBulletAngle(int numberOfBullets, int index, int maxSideAngle)
        {
            if (numberOfBullets == 1)
            {
                return 0f;
            }

            // Interpolation entre le point le plus a gauche et le point le plus a droite
            float t = (float)index / numberOfBullets;
            return Mathf.Lerp(-maxSideAngle, maxSideAngle, t);
        }

        private static ShotParams SetupBulletPosition(ShotPatternData pattern, float currentAngle, float spawnDistance, ShotParams parameters, int shotCount)
        {
            Quaternion rotation2D = Quaternion.Euler(0, 0, currentAngle + pattern.startingRotation + (shotCount * pattern.constantRotation));
            
            Vector2 direction = rotation2D * parameters.SpawnDirection;
            Vector2 spawnPosition = (Vector2)parameters.Instigator.transform.position + (direction.normalized * spawnDistance);
            
            return parameters with { SpawnDirection = direction, SpawnPosition = spawnPosition };
        }
    }
}
