using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BoleteHell.Code.Arsenal.ShotPatterns
{
    [Serializable]
    public class ShotPatternData
    {
        [Header("General")]
        [Tooltip("Number of bullets in a single shot")] 
        [Range(1, 50)]
        public int numberOfBulletShot = 1;
    
        [Tooltip("Angle in which the bullets will be shot")]
        [Range(0, 360)] [Unit(Units.Degree)]
        public int maxAngleRange;
    
        [Tooltip("Rotation of the pattern once at the start")]
        [Range(0, 360)] [Unit(Units.Degree)]
        public int startingRotation;

        [Tooltip("Rotation value every shot")]
        [Unit(Units.Degree)]
        public float constantRotation;

        public bool followOwnerLookingDirection = true;
    }
}
