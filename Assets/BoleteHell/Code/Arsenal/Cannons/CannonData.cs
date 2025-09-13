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
        
        [Tooltip("Delay after a shot before being able to shoot again")] 
        [SerializeField] [Min(0)] [Unit(Units.Second)]
        public float cooldown;
        
        [Tooltip("Time during which the shot input must be pressed before allowing fire")] 
        [SerializeField] [ShowIf(nameof(firingType), FiringTypes.Charged)] [Unit(Units.Second)] [Min(0)]
        public float chargeTime;
        
        [SerializeField] [Min(0)]
        public int maxNumberOfBounces = 10;
        
        [SerializeField] [ShowIf(nameof(firingType), FiringTypes.Charged)] [Unit(Units.Meter)] [Min(0)]
        public float maxRayDistance = 10;
        
        [SerializeField]
        private bool useCustomLifetime = false;
        
        [Tooltip("Time before the projectile despawns.")]
        [SerializeField] [ShowIf(nameof(useCustomLifetime))] [Unit(Units.Second)] [Min(0)]
        private float lifetime = 5.0f;
        
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
