using Lasers;
using UnityEngine;

public class ScriptableObjectCloner : MonoBehaviour
{
    
    
    public static T CloneScriptableObject<T>(T original) where T : ScriptableObject
    {
        T clone = Instantiate(original);
        return clone;
    }

}
