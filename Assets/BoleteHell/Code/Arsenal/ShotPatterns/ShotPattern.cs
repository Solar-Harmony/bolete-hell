using System;
using System.Collections;
using System.Collections.Generic;
using BoleteHell.Code.Arsenal.Cannons;
using BoleteHell.Code.Utils;
using UnityEngine;
using Zenject;

namespace BoleteHell.Code.Arsenal.ShotPatterns
{
    public class ProjectileLaunchData
    {
        public Vector2 StartPosition;
        public Vector2 Direction;
        public ProjectileLaunchData(Vector2 position, Vector2 direction)
        {
            StartPosition = position;
            Direction = direction;
        }
    }

    public class ShotPattern
    {
        public List<ProjectileLaunchData> Fire(ShotPatternData pattern, Vector2 spawnPoint, Vector2 initialDirection, GameObject instigator, int shotCount)
        {
            float spawnDistance = Vector3.Distance(instigator.transform.position, spawnPoint);
            //_currentRotation += pattern.constantRotation;
            //Angle maximal d'un coté
            int maxSideAngle = pattern.maxAngleRange / 2;
            List<ProjectileLaunchData> projectileData = new  List<ProjectileLaunchData>();
            
            for (int i = 0; i < pattern.numberOfBulletShot; i++)
            {
                SetupBulletPosition(pattern, i, maxSideAngle, spawnDistance, initialDirection, instigator, shotCount, 
                    out Vector2 direction, out Vector2 spawnPosition);
                
                projectileData.Add(new ProjectileLaunchData(spawnPosition, direction));
            }

            return projectileData;
        }

        private void SetupBulletPosition(ShotPatternData pattern, int i, int maxSideAngle,
            float spawnDistance, Vector2 initialDirection, GameObject instigator, int shotCount, out Vector2 direction, out Vector2 spawnPosition)
        {
            float currentAngle = SetBulletAngle(pattern.numberOfBulletShot, i, maxSideAngle);

            Quaternion rotation2D = Quaternion.Euler(0, 0, currentAngle + pattern.startingRotation + (shotCount * pattern.constantRotation));
            
            direction = rotation2D * initialDirection;
            spawnPosition = (Vector2)instigator.transform.position + (direction.normalized * spawnDistance);
        }

        private static float SetBulletAngle(int numberOfBullets, int i, int maxSideAngle)
        {
            float currentAngle;
            if (numberOfBullets == 1)
            {
                currentAngle = 0f; 
            }
            else
            {
                //Interpolation entre le point le plus a gauche et le point le plus a droite
                float t = (float)i / (numberOfBullets);
                currentAngle = Mathf.Lerp(-maxSideAngle, maxSideAngle, t);
            }

            return currentAngle;
        }
    }
}
