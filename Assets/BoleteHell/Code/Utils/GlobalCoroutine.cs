using System.Collections;
using UnityEngine;

namespace BoleteHell.Code.Utils
{
    public class GlobalCoroutine : MonoBehaviour, IGlobalCoroutine
    {
        public Coroutine Launch(IEnumerator routine)
        {
            return StartCoroutine(routine);
        }
    }
}