using UnityEngine;

/// <summary>
/// Script qui permet de différentier si l'objet toucher est une ligne et quoi faire selon la ligne touché
/// </summary>
public class Line : MonoBehaviour
{
    [SerializeField]private LineSO _lineInfo;

    public void SetLineInfo(LineSO lineInfo)
    {
        _lineInfo = lineInfo;
    }

    public void OnRayHitLine(Vector3 incomingDirection,RaycastHit2D hitPoint,Ray ray)
    {
        if(_lineInfo.Equals(null))
            Debug.LogError($"{name} has no lineInfo setup it should be set before calling this");
        
        _lineInfo.OnRayHit(incomingDirection, hitPoint,ray);
    }

}
