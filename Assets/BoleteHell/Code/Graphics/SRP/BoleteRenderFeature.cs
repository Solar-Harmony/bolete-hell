using System;
using BoleteHell.Code.Rendering.SDF;
using BoleteHell.Code.Utils;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace BoleteHell.Code.Graphics.SRP
{
    [Serializable]
    public class BoleteRenderingSettings
    {
        [Tooltip("Rendering layer mask for the obstacles silhouette.")]
        [ValidateInput(nameof(ValidateRenderLayer), "Invalid rendering layer name.")]
        public string RenderingLayerMaskName;
        private bool ValidateRenderLayer(string name) => RenderingLayerMask.NameToRenderingLayer(name) != -1;
        
        [Tooltip("Reference resolution height for resolution-independent effects.")]
        public float ReferenceHeight = 1080f;
        
        [ToggleGroup(nameof(EnableSunShadow), "Screen-space sun shadow")]
        public bool EnableSunShadow = true;

        [ToggleGroup(nameof(EnableSunShadow))] [AnglePicker]
        public Vector2 SunDirection = new(0.1f, 0.1f);
        
        [ToggleGroup(nameof(EnableSunShadow))] 
        [Tooltip("Step size for shadow raymarching.")]
        public float SunShadowStepSize = 0.01f;
        
        [ToggleGroup(nameof(EnableSunShadow))]
        [Tooltip("Intensity of the sun shadows.")]
        public float SunShadowIntensity = 0.5f;
        
        [ToggleGroup(nameof(EnableSunShadow))]
        [Tooltip("Softness of the sun shadows.")]
        public float SunShadowSoftness = 0.0f;
    }
    
    public class BoleteRenderFeature : ScriptableRendererFeature
    {
        [SerializeField]
        private BoleteRenderingSettings _settings = new();
        
        private Material _silhouetteMaterial;
        private ObstaclesSilhouettePass _silhouettePass;
        private Material _fakeAOMaterial;
        private FakeAOPass _fakeAOPass;
        private Material _fakeSunShadowMaterial;
        private FakeSunShadowPass _fakeSunShadowPass;

        public override void Create()
        {
            _silhouetteMaterial = CoreUtils.CreateEngineMaterial(Shader.Find("Bolete Hell/Simple White"));
            _silhouettePass = new ObstaclesSilhouettePass(_settings.RenderingLayerMaskName, _silhouetteMaterial)
            {
                renderPassEvent = RenderPassEvent.BeforeRendering
            };

            _fakeAOMaterial = CoreUtils.CreateEngineMaterial(Shader.Find("Bolete Hell/Fake Ambient Occlusion"));
            _fakeAOPass = new FakeAOPass(_fakeAOMaterial, _settings.ReferenceHeight)
            {
                renderPassEvent = RenderPassEvent.AfterRenderingTransparents
            };
            
            _fakeSunShadowMaterial = CoreUtils.CreateEngineMaterial(Shader.Find("Bolete Hell/Fake Sun Shadow"));
            _fakeSunShadowPass = new FakeSunShadowPass(_fakeSunShadowMaterial, _settings.SunDirection, _settings.SunShadowStepSize, _settings.SunShadowIntensity, _settings.SunShadowSoftness)
            {
                renderPassEvent = RenderPassEvent.AfterRenderingTransparents
            };
        }
        
        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            renderer.EnqueuePass(_silhouettePass);
            renderer.EnqueuePass(_fakeAOPass);
            if (_settings.EnableSunShadow)
                renderer.EnqueuePass(_fakeSunShadowPass);
        }

        protected override void Dispose(bool disposing)
        {
            CoreUtils.Destroy(_silhouetteMaterial);
            CoreUtils.Destroy(_fakeAOMaterial);
            CoreUtils.Destroy(_fakeSunShadowMaterial);
        }
    }
}
