using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace BoleteHell.Code.Rendering.SDF
{
    [Serializable]
    public class SDFRenderingSettings
    {
        [ValidateInput(nameof(ValidateRenderLayer), "Invalid rendering layer name.")]
        public string renderingLayerMaskName;
        
        [Range(1, 10f)] 
        public float tempBlurStrength = 1f;

        private bool ValidateRenderLayer(string name) => RenderingLayerMask.NameToRenderingLayer(name) != -1;
    }
    
    public class SDFRenderFeature : ScriptableRendererFeature
    {
        [SerializeField]
        private SDFRenderingSettings settings;
        
        private ObstaclesSilhouettePass _silhouettePass;
        
        private EdgeDetectionRenderPass _edgeDetectionPass;
        private Material _edgeDetectionMaterial;
        private Material _blurMaterial;

        public override void Create()
        {
            InitEdgeDetectionMaterial();
            InitBlurMaterial();
            
            _silhouettePass = new ObstaclesSilhouettePass(settings.renderingLayerMaskName)
            {
                renderPassEvent = RenderPassEvent.AfterRenderingTransparents
            };
            
            _edgeDetectionPass = new EdgeDetectionRenderPass(_edgeDetectionMaterial, _blurMaterial, settings.tempBlurStrength, settings.tempBlurStrength)
            {
                renderPassEvent = RenderPassEvent.AfterRenderingTransparents
            };
        }
        
        private void InitEdgeDetectionMaterial()
        {
            var shader = Shader.Find("CustomEffects/EdgeSDF");
            _edgeDetectionMaterial = CoreUtils.CreateEngineMaterial(shader);
        }
        
        private void InitBlurMaterial()
        {
            var shader = Shader.Find("CustomEffects/GaussianBlur");
            _blurMaterial = CoreUtils.CreateEngineMaterial(shader);
        }
        
        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            renderer.EnqueuePass(_silhouettePass);
            renderer.EnqueuePass(_edgeDetectionPass);
        }

        protected override void Dispose(bool disposing)
        {
            CoreUtils.Destroy(_edgeDetectionMaterial);
            CoreUtils.Destroy(_blurMaterial);
        }
    }
}
