using System;
using BoleteHell.Code.Rendering.SDF;
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
    }
    
    public class BoleteRenderFeature : ScriptableRendererFeature
    {
        [SerializeField]
        private BoleteRenderingSettings _settings = new();
        
        private Material _silhouetteMaterial;
        private ObstaclesSilhouettePass _silhouettePass;
        private Material _fakeAOMaterial;
        private FakeAOPass _fakeAOPass;

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
        }
        
        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            renderer.EnqueuePass(_silhouettePass);
            renderer.EnqueuePass(_fakeAOPass);
        }

        protected override void Dispose(bool disposing)
        {
            CoreUtils.Destroy(_fakeAOMaterial);
        }
    }
}
