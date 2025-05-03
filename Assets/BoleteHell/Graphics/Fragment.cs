using System;
using UnityEngine;

namespace BoleteHell.Graphics
{
    public class Fragment : MonoBehaviour
    {
        private void Start()
        {
            Destroy(gameObject, 5f);
        }
    }
}