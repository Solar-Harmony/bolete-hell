using System;
using UnityEngine;

namespace BoleteHell.Code.Arsenal.ShotPatterns
{
    [Serializable]
    public class ShotPatternData
    {
        [Header("General")]
        [Tooltip("Number of bullets in a single shot")]
        public int numberOfBulletShot;
    
        [Tooltip("Angle in which the bullets will be shot")]
        [Range(0,360)]
        public int maxAngleRange;
    
        [Tooltip("Rotation of the pattern once at the start")]
        public int startingRotation;

        [Tooltip("Rotation value every shot")]
        public int constantRotation;
    
        [Header("Burst parameters")]
    
        [Tooltip("Number of shots in a burst")]
        [Range(1,3)]
        public int burstShotCount = 1;
    
        [Tooltip("FireRate between individual shots in a burst")]
        public float burstShotCooldown;
    }
}
