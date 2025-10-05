using BoleteHell.Code.Arsenal.Rays.RayLogic;
using BoleteHell.Code.Gameplay.Character;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BoleteHell.Code.Arsenal.RayData
{
    [CreateAssetMenu(fileName = "LaserData", menuName = "BoleteHell/Arsenal/Laser Data", order = -100)]
    public class LaserData : ScriptableObject
    {
        [field: SerializeField] 
        public Color Color { get; private set; } = Color.white;

        [field: SerializeField] [Tooltip("La valeur ne devrait pas dépasser la valeur de refraction du shield sinon ça créé des problèmes")]
        public float LightRefractiveIndex { get; private set; } = 1.33f;
        
        [Required] [field: SerializeReference] 
        public RayHitLogic Logic { get; private set; }
        
        [SerializeField]
        [Tooltip("Détermine quel côté du conflit le laser va affecter")]
        public AffectedSide affectedSide = AffectedSide.Enemies;

        [field: SerializeField] 
        public int baseDamage { get; private set; }

        [field: SerializeField] 
        public float MovementSpeed { get; set; }
    }
}