using BoleteHell.RayCannon;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Data.Cannons
{
    [CreateAssetMenu(fileName = "RayCannonData", menuName = "Scriptable Objects/RayCannonData")]
    public class RayCannonData : ScriptableObject
    {
        [SerializeField] public FiringTypes firingType;
        [SerializeField] public float timeBetweenShots = 0.5f;
        [SerializeField] public float lifeTime = 0.1f;
        [SerializeField] public int maxNumberOfBounces = 10;
        [ShowIf("firingType",FiringTypes.Charged)]
        [SerializeField] public float maxRayDistance = 10;
    }
}
