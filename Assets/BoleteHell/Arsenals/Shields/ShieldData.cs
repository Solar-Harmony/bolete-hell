using BoleteHell.Arsenals.Shields.ShieldsLogic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BoleteHell.Arsenals.Shields
{
    /// <summary>
    ///     Classe qui permet de déterminer les informations spécifique a un shield
    /// </summary>
    [CreateAssetMenu(fileName = "ShieldData", menuName = "BoleteHell/Arsenal/Shield Data")]
    public class ShieldData : ScriptableObject
    {
        [field: SerializeField] 
        public Color color { get; set; }

        [Required] [field:SerializeReference] 
        public IShieldHitLogic onHitLogic { get; private set; }
        
        [SerializeField]
        public float despawnTime = 3f;
        
        [field: SerializeField]
        [MinValue(0f)]
        public float EnergyCostPerCm { get; set; } = 10f;
    }
}