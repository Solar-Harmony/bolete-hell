using System;
using BoleteHell.Rendering.SRP.FakeAO;
using BoleteHell.Rendering.SRP.Silhouette;
using BoleteHell.Rendering.SRP.SunShadows;
using BoleteHell.Utils;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering.Universal;

// extensions du render pipeline
namespace BoleteHell.Rendering.SRP
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
        
        [ToggleGroup(nameof(EnableFakeAO), "Screen-space \"ambient occlusion\"")]
        public bool EnableFakeAO = true;
        
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

        [ToggleGroup(nameof(EnableFakeAO))]
        [Tooltip("Radius for ambient occlusion sampling.")]
        public float FakeAORadius = 10.0f;
        
        private const string _groupName = "Materials";
        
        [FoldoutGroup(_groupName)]
        [Required] [SerializeField]
        public Material SilhouetteMaterial;
        
        [FoldoutGroup(_groupName)]
        [Required] [SerializeField]
        public Material FakeAOMaterial;
        
        [FoldoutGroup(_groupName)]
        [Required] [SerializeField]
        public Material FakeSunShadowMaterial;
    }
    
    public class BoleteRenderFeature : ScriptableRendererFeature
    {
        [SerializeField]
        private BoleteRenderingSettings _settings = new();
        
        private ObstaclesSilhouettePass _silhouettePass;
        private FakeAOPass _fakeAOPass;
        private FakeSunShadowPass _fakeSunShadowPass;

        private bool _initialized = false;

        public override void Create()
        {
            if (!_settings.SilhouetteMaterial || !_settings.FakeAOMaterial || !_settings.FakeSunShadowMaterial)
                return;
                
            _silhouettePass = new ObstaclesSilhouettePass(_settings.RenderingLayerMaskName, _settings.SilhouetteMaterial)
            {
                renderPassEvent = RenderPassEvent.BeforeRendering
            };

            _fakeAOPass = new FakeAOPass(_settings.FakeAOMaterial, _settings.ReferenceHeight, _settings.FakeAORadius)
            {
                renderPassEvent = RenderPassEvent.AfterRenderingTransparents
            };
            
            _fakeSunShadowPass = new FakeSunShadowPass(_settings.FakeSunShadowMaterial, _settings.SunDirection, _settings.SunShadowStepSize, _settings.SunShadowIntensity, _settings.SunShadowSoftness)
            {
                renderPassEvent = RenderPassEvent.AfterRenderingTransparents
            };
            
            _initialized = true;
        }
        
        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (!_initialized)
                return;
            
            renderer.EnqueuePass(_silhouettePass);
            
            if (_settings.EnableFakeAO)
                renderer.EnqueuePass(_fakeAOPass);
            
            if (_settings.EnableSunShadow)
                renderer.EnqueuePass(_fakeSunShadowPass);
        }
    }
}
