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
        private Material _jfaMaterial;
        private Material _combineMaterial;
        private Material _copyMaterial;

        public override void Create()
        {
            InitCopyMaterial();
            InitJFAMaterial();
            InitCombineMaterial();
            
            _silhouettePass = new ObstaclesSilhouettePass(settings.renderingLayerMaskName)
            {
                renderPassEvent = RenderPassEvent.AfterRenderingTransparents
            };
            
            _edgeDetectionPass = new EdgeDetectionRenderPass(_copyMaterial, _jfaMaterial, _combineMaterial, settings.tempBlurStrength)
            {
                renderPassEvent = RenderPassEvent.AfterRenderingTransparents
            };
        }
        
        private void InitCopyMaterial()
        {
            var shader = Shader.Find("CustomEffects/SimpleCopy");
            _copyMaterial = CoreUtils.CreateEngineMaterial(shader);
        }
        
        private void InitJFAMaterial()
        {
            var shader = Shader.Find("CustomEffects/JumpFloodSDF");
            _jfaMaterial = CoreUtils.CreateEngineMaterial(shader);
        }
        
        private void InitCombineMaterial()
        {
            var shader = Shader.Find("CustomEffects/SDFCombine");
            _combineMaterial = CoreUtils.CreateEngineMaterial(shader);
        }
        
        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            renderer.EnqueuePass(_silhouettePass);
            renderer.EnqueuePass(_edgeDetectionPass);
        }

        protected override void Dispose(bool disposing)
        {
            CoreUtils.Destroy(_copyMaterial);
            CoreUtils.Destroy(_jfaMaterial);
            CoreUtils.Destroy(_combineMaterial);
        }
    }
}
