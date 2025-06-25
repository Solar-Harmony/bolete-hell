using BoleteHell.Code.Arsenal.Shields.ShieldsLogic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BoleteHell.Code.Arsenal.Shields
{
    /// <summary>
    ///     Classe qui permet de déterminer les informations spécifique a un shield
    /// </summary>
    [CreateAssetMenu(fileName = "ShieldData", menuName = "BoleteHell/Arsenal/Shield Data")]
    public class ShieldData : ScriptableObject
    {
        [SerializeField] 
        private Color color;
        
        [Required] [SerializeReference] 
        private IShieldHitLogic onHitLogic;

        [SerializeField] 
        private GameObject shieldPreview;
        
        private ShieldPreviewDrawer lineDrawer;
        
        public void StartLine()
        {
            var obj = Instantiate(shieldPreview);
            lineDrawer = obj.GetComponent<ShieldPreviewDrawer>();
        }

        public void DrawShieldPreview(Vector3 nextPos)
        {
            lineDrawer.DrawPreview(nextPos);
        }

        public void FinishLine()
        {
            lineDrawer.FinishLine(this);
        }

        public Vector2 OnRayHit(Vector2 incomingDirection, RaycastHit2D hitPoint, float lightRefractiveIndice)
        {
            return onHitLogic.ExecuteRay(incomingDirection, hitPoint, lightRefractiveIndice);
        }
    }
}