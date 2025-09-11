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
        
        [Required] [SerializeReference] 
        private IShieldHitLogic onHitLogic;

        [SerializeField] 
        private GameObject shieldPreview;
        
        [SerializeField]
        public float despawnTime = 3f;
        
        private ShieldPreviewDrawer lineDrawer;
        
        [SerializeReference] [Required]
        private ShieldEffect statusEffectConfig;

        [Inject]
        private IStatusEffectService _statusEffectService;
        
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

        public Vector2 OnRayHit(Vector2 incomingDirection, RaycastHit2D hitPoint, LaserCombo laser, GameObject instigator)
        {
            //TODO: Pourrait surement ajouter une faction aux shield comme ca si les ennemis créé un shield ça pourrait affecter
            //Les tirs des ennemis qui passe a travers le shield (ou si jamais le joueur peut avoir des alliés)
            if (instigator == _player.gameObject)
            {
                //Peut avoir un probleme si jamais on a un buff de dégat qui n'est pas un poucentage car ca va être ajouter a chaque laser du combo
                //(si un laser a 3 couleurs lui jouter 10 dégat donne réellement 30 dégat)
                foreach (LaserData laserData in laser.Datas)
                {
                    _statusEffectService.AddStatusEffect(laserData.Logic, statusEffectConfig);
                }
            }
            
            return onHitLogic.ExecuteRay(incomingDirection, hitPoint, laser.CombinedRefractiveIndex);
        }
    }
}