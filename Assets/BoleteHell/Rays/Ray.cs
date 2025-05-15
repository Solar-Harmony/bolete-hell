using System.Collections.Generic;
using Lasers.RayLogic;
using Shields;
using UnityEngine;

namespace Lasers
{
    [CreateAssetMenu(fileName = "Ray", menuName = "Scriptable Objects/Ray/RayInfo")]
    public class Ray : ScriptableObject
    {
        [field: SerializeField] public Color Color { get; private set; }
        [Tooltip("La valeur ne devrait pas dépasser la valeur de refraction du shield sinon ça créé des problèmes")]
        [field: SerializeField] public float LightRefractiveIndex { get; private set; }
        [field: SerializeReference] public RayHitLogic logic { get; private set; }
        [field:SerializeField] public int maxNumberOfBounces { get; private set; } = 10;
        [field:SerializeField] public float maxRayDistance { get; private set; } = 10;
        [field:SerializeField] public float rayWidth { get; private set; } = 0.2f;
        [Header("Specific to projectile lasers")]
        [field:SerializeField] public float raylength { get; private set; } = 0.3f;
    }
}