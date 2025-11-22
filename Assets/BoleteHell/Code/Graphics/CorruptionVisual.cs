using System;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

namespace BoleteHell.Code.Graphics
{
    [Serializable]
    public class CorruptionVisualSettings
    {
        public Color ColorWhenHealthy = new(0.2f, 0.76f, 0.31f);
        public Color ColorWhenCorrupted = new(0.2f, 0.07f, 0.26f);
        public int TextureSize = 1024;
    }
    
    [RequireComponent(typeof(Renderer))]
    public class CorruptionVisual : MonoBehaviour
    {
        private static readonly int _corruptionTex = Shader.PropertyToID("_CorruptionTex");
        private Renderer _renderer;
        
        [SerializeField]
        private CorruptionVisualSettings _settings;
        
        private void Awake()
        {
            _renderer = GetComponent<Renderer>();
            
            GenerateInitialTexture();
        }

        private Texture2D _mask;

        private void GenerateInitialTexture()
        {
            _mask = new Texture2D(_settings.TextureSize, _settings.TextureSize, GraphicsFormat.R8_UNorm, TextureCreationFlags.None)
            {
                wrapMode = TextureWrapMode.Clamp,
                filterMode = FilterMode.Bilinear
            };
            
            Color[] pixels = new Color[_settings.TextureSize * _settings.TextureSize];
            for (int i = 0; i < pixels.Length; i++)
                pixels[i] = Color.white;
            
            _mask.SetPixels(pixels);
            _mask.Apply();
            
            _renderer.material.SetTexture(_corruptionTex, _mask);
        }

        public void PaintCleanCircle(Vector3 worldPos, Transform terrainTransform, float radius, float falloff)
        {
            // Step 1: Convert world → local
            Vector3 localPos = terrainTransform.InverseTransformPoint(worldPos);

            // Step 2: Normalize to UV (assuming terrain mesh spans 0–1 in local XZ)
            // Adjust depending on your mesh orientation
            Bounds bounds = terrainTransform.GetComponent<MeshRenderer>().bounds;
            Vector3 min = terrainTransform.InverseTransformPoint(bounds.min);
            Vector3 max = terrainTransform.InverseTransformPoint(bounds.max);

            float u = Mathf.InverseLerp(min.x, max.x, localPos.x);
            float v = Mathf.InverseLerp(min.z, max.z, localPos.z);

            // Step 3: Convert UV → pixel coords
            int cx = (int)(u * _mask.width);
            int cy = (int)(v * _mask.height);

            // Step 4: Paint circle
            int radPixels = (int)(radius * _mask.width); // radius in texture space
            for (int y = -radPixels; y <= radPixels; y++)
            {
                for (int x = -radPixels; x <= radPixels; x++)
                {
                    int px = cx + x;
                    int py = cy + y;
                    if (px < 0 || px >= _mask.width || py < 0 || py >= _mask.height) continue;

                    float dist = Mathf.Sqrt(x * x + y * y);
                    float influence = Mathf.Clamp01(1f - Mathf.InverseLerp(radPixels - falloff, radPixels, dist));

                    Color current = _mask.GetPixel(px, py);
                    float newValue = Mathf.Max(current.r, influence);
                    _mask.SetPixel(px, py, new Color(newValue, 0, 0));
                }
            }
            _mask.Apply();
        }

        private void OnDestroy()
        {
            if (_mask)
            {
                Destroy(_mask);
            }
        }
    }
}