using System;
using UnityEngine;

namespace Utils
{
    [RequireComponent(typeof(Renderer))]
    public class SDFGenerator : MonoBehaviour
    {
        public int baseResolution = 64;
        public float padding = 4.0f;
        public int blurRadius = 2;
        public bool useSubsampling = true;
    }
}