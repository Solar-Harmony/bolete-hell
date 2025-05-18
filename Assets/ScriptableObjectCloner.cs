using Lasers;
using UnityEngine;

public class ScriptableObjectCloner : MonoBehaviour
{
    
    
    public static T CloneScriptableObject<T>(T original) where T : ScriptableObject
    {
        if (!original)
        {
            Debug.LogError("Tried to clone a null object");
        }

        T clone = Instantiate(original);
        return clone;
    }

}
