using UnityEngine;

namespace BoleteHell.Code.Utils
{
    public class ObjectInstantiator : MonoBehaviour
    {
        //Permet de cloner des scriptableObjects dans des classes qui ne sont pas des monobehaviour
        public static T CloneScriptableObject<T>(T original) where T : ScriptableObject
        {
            if (!original)
            {
                Debug.LogError("Tried to clone a null object");
            }

            T clone = Instantiate(original);
            return clone;
        }
    
        public static void InstantiateObjectForAmountOfTime(GameObject obj,Vector2 position, float time)
        {
            GameObject instantiatedObj = Instantiate(obj,position,Quaternion.identity);
            Destroy(instantiatedObj, time); 
        }
    }
}
