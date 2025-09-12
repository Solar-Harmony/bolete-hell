using BoleteHell.Code.Arsenal.RayData;
using BoleteHell.Code.Arsenal.Shields.ShieldsLogic;
using BoleteHell.Code.Gameplay.Character;
using BoleteHell.Code.Gameplay.Damage.Effects;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace BoleteHell.Code.Arsenal.Shields
{
    /// <summary>
    ///     Classe qui permet de déterminer les informations spécifique a un shield
    /// </summary>
    [CreateAssetMenu(fileName = "ShieldData", menuName = "BoleteHell/Arsenal/Shield Data")]
    public class ShieldData : ScriptableObject, IRequestManualInject
    {
        [field: SerializeField] 
        public Color color { get; set; }

        [Required] [field:SerializeReference] 
        public IShieldHitLogic onHitLogic { get; private set; }

        [SerializeField] 
        private GameObject shieldPreview;
        
        [SerializeField]
        public float despawnTime = 3f;
        
        private ShieldPreviewDrawer lineDrawer;
        
        [field:SerializeReference] [Required]
        public StatusEffectConfig statusEffectConfig { get; private set; }
        
        [field: SerializeField]
        [MinValue(0f)]
        public float EnergyCostPerCm { get; set; } = 10f;

        private Player _player;
        private bool _isInjected;
        bool IRequestManualInject.IsInjected
        {
            get => _isInjected;
            set => _isInjected = value;
        }

        public void StartLine()
        {
            var obj = Instantiate(shieldPreview);
            lineDrawer = obj.GetComponent<ShieldPreviewDrawer>();
            _player = FindFirstObjectByType<Player>();
        }

        public void DrawShieldPreview(Vector3 nextPos)
        {
            lineDrawer.Initialize(_player, this);
            lineDrawer.DrawPreview(nextPos);
        }

        public void FinishLine()
        {
            lineDrawer.FinishLine(this);
        }
    }
}