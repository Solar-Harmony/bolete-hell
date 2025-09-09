using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BoleteHell.Code.Arsenal.Cannons
{
    [CreateAssetMenu(fileName = "CannonData", menuName = "BoleteHell/Arsenal/Cannon", order = -100)]
    public class CannonData : ScriptableObject
    { 
        [SerializeField] 
        public FiringTypes firingType;
        
        [SerializeField] [Tooltip("Time between each shot")] [Min(0)]
        public float rateOfFire;
        
        // TODO: This is meaningless for Beam projectiles
        [SerializeField] [Unit(Units.MetersPerSecond)]
        public float projectileSpeed = 10.0f;
        
        [SerializeField] [Min(0)]
        public int maxNumberOfBounces = 10;
        
        [SerializeField] [ShowIf(nameof(firingType), FiringTypes.Charged)] [Unit(Units.Meter)] [Min(0)]
        public float maxRayDistance = 10;
        
        [SerializeField]
        private bool useCustomLifetime = false;
        
        [SerializeField] [ShowIf(nameof(useCustomLifetime))] [Unit(Units.Second)] [Min(0)]
        private float lifetime = 5.0f;
        
        [SerializeField] [ShowIf(nameof(firingType), FiringTypes.Charged)] [Tooltip("Time for the shot to be charged")] [Unit(Units.Second)] [Min(0)]
        public float chargeTime;
        
        public float Lifetime => useCustomLifetime ? lifetime : firingType switch
        {
            FiringTypes.Automatic => 20.0f,
            FiringTypes.Charged => 0.1f,
            _ => throw new ArgumentOutOfRangeException()
        };

        public bool WaitBeforeFiring => firingType switch
        {
            FiringTypes.Automatic => false,
            FiringTypes.Charged => true,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}
