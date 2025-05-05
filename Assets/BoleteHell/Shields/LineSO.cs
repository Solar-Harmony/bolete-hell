using Shields.ShieldsLogic;
using UnityEngine;

namespace Shields
{
    internal enum LineType
    {
        Refract,
        Reflect,
        Disperse
    }

    /// <summary>
    ///     Classe qui permet de déterminer les informations spécifique a un shield
    /// </summary>
    [CreateAssetMenu(fileName = "LineSO", menuName = "LineData", order = 0)]
    public class LineSO : ScriptableObject
    {
        [SerializeField] private Color color;


        [SerializeReference] private ILineHitLogic onHitLogic;

        [SerializeField] private GameObject shieldPreview;
        private ShieldPreviewDrawer lineDrawer;

        //Doing this so they all have a reference to the same object and so it can't be changed from editor

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

        public Vector3 OnRayHit(Vector3 incomingDirection, RaycastHit2D hitPoint, float lightRefractiveIndice)
        {
            return onHitLogic.ExecuteRay(incomingDirection, hitPoint, lightRefractiveIndice);
        }
    }
}