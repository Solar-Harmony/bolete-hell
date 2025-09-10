using System.Collections;
using UnityEngine;

namespace BoleteHell.Code.Utils
{
    public interface IGlobalCoroutine
    {
        Coroutine Launch(IEnumerator routine);
    } 
}