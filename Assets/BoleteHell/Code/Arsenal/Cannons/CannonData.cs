using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BoleteHell.Code.Arsenal.Cannons
{
    //Peut-être déplacer l'information dans les bulletPatterns directement
    //Mais le fait de garder le firingType séparer du BulletPatternData permet d'utiliser le même BulletPatternData pour n'importe quel type de projectile
    //Sans avoir a faire un BulletPatternData par type
    [CreateAssetMenu(fileName = "CannonData", menuName = "BoleteHell/Arsenal/Cannon", order = -100)]
    public class CannonData : ScriptableObject
    { 
        [SerializeField] public FiringTypes firingType;
        [Tooltip("Time between each shot")]
        [SerializeField] public float rateOfFire;
        
        [SerializeField] public float projectileSpeed = 10f;
        
        //Lifetime of the projectile/beam
        public float LifeTime {
            get
            {
                return firingType switch
                {
                    FiringTypes.Automatic => 2f,
                    FiringTypes.Charged => 0.1f,
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
        }
        public bool WaitBeforeFiring {
            get
            {
                return firingType switch
                {
                    FiringTypes.Automatic => false,
                    FiringTypes.Charged => true,
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
        }
        
        [SerializeField] public int maxNumberOfBounces = 10;
        [ShowIf("firingType",FiringTypes.Charged)]
        [SerializeField] public float maxRayDistance = 10;
    }
}
