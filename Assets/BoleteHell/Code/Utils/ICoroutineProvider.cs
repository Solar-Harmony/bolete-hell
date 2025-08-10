using System.Collections;
using UnityEngine;

namespace BoleteHell.Code.Utils
{
    public interface ICoroutineProvider
    {
        Coroutine StartCoroutine(IEnumerator routine);
        void StopCoroutine(Coroutine coroutine);
    }
}