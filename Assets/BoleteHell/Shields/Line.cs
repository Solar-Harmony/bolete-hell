using UnityEngine;

namespace Shields
{
    /// <summary>
    ///     Script qui permet de rien faire
    ///     jk il permet de différentier si l'objet toucher est une ligne et quoi faire selon la ligne touché
    /// </summary>
    public class Line : MonoBehaviour
    {
        [SerializeField] private LineSO _lineInfo;

        public void SetLineInfo(LineSO lineInfo)
        {
            _lineInfo = lineInfo;
        }

        public Vector3 OnRayHitLine(Vector3 incomingDirection, RaycastHit2D hitPoint, float lightRefractiveIndice)
        {
            if (_lineInfo.Equals(null))
                Debug.LogError($"{name} has no lineInfo setup it should be set before calling this");

            return _lineInfo.OnRayHit(incomingDirection, hitPoint, lightRefractiveIndice);
        }
    }
}