using System.Collections;
using UnityEngine;

namespace _BoleteHell.Code.Utils
{
    public class GlobalCoroutine : MonoBehaviour
    {
        private static GlobalCoroutine _instance;
        private static GlobalCoroutine Instance
        {
            get
            {
                if (_instance) 
                    return _instance;
                
                var obj = new GameObject("CoroutineRunner");
                _instance = obj.AddComponent<GlobalCoroutine>();
                DontDestroyOnLoad(obj);
                
                return _instance;
            }
        }
        
        public static Coroutine Launch(IEnumerator routine)
        {
            return Instance.StartCoroutine(routine);
        }
    }
}