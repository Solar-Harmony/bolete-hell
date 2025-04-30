using UnityEngine;

public class Line : MonoBehaviour
{
    private LineSO _lineInfo;

    public void SetLineInfo(LineSO lineInfo)
    {
        _lineInfo = lineInfo;
    }

    public void OnRayHitLine(Vector3 incomingDirection,RaycastHit hitPoint,Ray ray)
    {
        if(_lineInfo.Equals(null))
            Debug.LogError($"{name} has no lineInfo setup");
        
        _lineInfo.OnRayHit(incomingDirection, hitPoint,ray);
    }

}
