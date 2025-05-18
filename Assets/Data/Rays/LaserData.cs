using System.Collections.Generic;
using Lasers.RayLogic;
using Shields;
using UnityEngine;

namespace Lasers
{
    public abstract class LaserData : ScriptableObject
    {
        [field: SerializeField] public Color Color { get; private set; }
        [Tooltip("La valeur ne devrait pas dépasser la valeur de refraction du shield sinon ça créé des problèmes")]
        [field: SerializeField] public float LightRefractiveIndex { get; private set; }
        [field: SerializeReference] public RayHitLogic logic { get; private set; }
        [field: SerializeField] public float baseDamage { get; private set; } = 10f;
        [field:SerializeField] public int maxNumberOfBounces { get; private set; } = 10;
        [field:SerializeField] public float maxRayDistance { get; private set; } = 10;
        [field:SerializeField] public float rayWidth { get; private set; } = 0.2f;
    }
}