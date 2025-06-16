using _BoleteHell.Code.ProjectileSystem.Rays.RayLogic;
using Lasers.RayLogic;
using UnityEngine;

namespace Lasers
{
    [CreateAssetMenu(fileName = "LaserData", menuName = "Scriptable Objects/LaserData", order = 0)]
    public class LaserData : ScriptableObject
    {
        [field: SerializeField] public Color Color { get; private set; }
        [Tooltip("La valeur ne devrait pas dépasser la valeur de refraction du shield sinon ça créé des problèmes")]
        [field: SerializeField] public float LightRefractiveIndex { get; private set; }
        [field: SerializeReference] public RayHitLogic Logic { get; private set; }
    }
}