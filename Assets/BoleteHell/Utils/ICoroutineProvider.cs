using System.Collections;
using UnityEngine;

namespace Utils.BoleteHell.Utils
{
    public interface ICoroutineProvider
    {
        Coroutine StartCoroutine(IEnumerator routine);
        void StopCoroutine(Coroutine coroutine);
    }
}