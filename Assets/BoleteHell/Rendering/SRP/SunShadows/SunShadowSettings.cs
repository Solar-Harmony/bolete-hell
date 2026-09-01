using System;
using BoleteHell.Utils;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BoleteHell.Rendering.SRP.SunShadows
{
    [Serializable]
    public class SunShadowSettings
    {
        [AnglePicker]
        [Tooltip("Sun direction for shadow raymarching.")]
        public Vector2 SunDirection = new(0.1f, 0.1f);

        [Tooltip("Step count for shadow raymarching.")]
        public int MaxSteps = 32;

        [Tooltip("Intensity of the sun shadows.")]
        public float Intensity = 0.5f;

        [Tooltip("Softness of the sun shadows.")]
        public float Softness = 0.0f;

        [Tooltip("Shadow length multiplier.")]
        public float LengthScale = 2.0f;

        [Tooltip("Shadow length multiplier.")]
        public float MaxLength = 12;
        
        [Tooltip("Height of the tallest shadow-casting object.")]
        public float MaxHeight = 5.0f;

        [Tooltip("Resolution of the shadows.")]
        public int MaxMaskResolution = 4096;
    }
}
