using UnityEngine;

namespace Shields
{
    public class Shield : MonoBehaviour
    {
        [SerializeField] private ShieldData _lineInfo;

        public void SetLineInfo(ShieldData lineInfo)
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